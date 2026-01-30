using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using PropostaService.Application.Services;
using PropostaService.Application.Interfaces;
using PropostaService.Domain.Entities;


namespace PropostaService.Tests
{
    public class PropostaServiceAppTests
    {
        private readonly Mock<IPropostaRepository> _propostaRepositoryMock = new();
        private readonly Mock<IIdempotencyRepository> _idempotencyRepositoryMock = new();
        private readonly Mock<ILogger<PropostaServiceApp>> _loggerMock = new();
        private readonly Mock<IRabbitMQService> _rabbitMQServiceMock = new();

        private PropostaServiceApp CriarService()
        {
            return new PropostaServiceApp(
                _propostaRepositoryMock.Object,
                _idempotencyRepositoryMock.Object,
                _loggerMock.Object,
                _rabbitMQServiceMock.Object
            );
        }

        [Fact]
        public async Task CriarPropostaAsync_DeveRetornarPropostaCriada()
        {
            // Arrange
            var service = CriarService();
            var nomeCliente = "João";
            var tipoSeguro = "Vida";
            var valor = 1000m;

            var propostaMock = new PropostaModel
            {
                Id = Guid.NewGuid(),
                NomeCliente = nomeCliente,
                TipoSeguro = tipoSeguro,
                Valor = valor,
                Status = StatusProposta.EmAnalise
            };

            var transactionMock = new Mock<PropostaService.Application.Interfaces.ITransaction>();
            transactionMock.Setup(t => t.CommitAsync()).Returns(Task.CompletedTask);
            transactionMock.Setup(t => t.RollbackAsync()).Returns(Task.CompletedTask);

            _propostaRepositoryMock
                .Setup(r => r.BeginTransactionAsync())
                .Returns(Task.FromResult(transactionMock.Object)); 
                                                                   

            _propostaRepositoryMock
                .Setup(r => r.AdicionarAsync(It.IsAny<PropostaModel>()))
                .ReturnsAsync(propostaMock);

            // Act
            var resultado = await service.CriarPropostaAsync(nomeCliente, tipoSeguro, valor);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(nomeCliente, resultado.NomeCliente);
            Assert.Equal(tipoSeguro, resultado.TipoSeguro);
            Assert.Equal(valor, resultado.Valor);

            _propostaRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<PropostaModel>()), Times.Once);
            transactionMock.Verify(t => t.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task AlterarStatusAsync_DeveAtualizarStatusEEnviarParaRabbitMQ()
        {
            // Arrange
            var service = CriarService();
            var propostaId = Guid.NewGuid();

            var propostaMock = new PropostaModel
            {
                Id = propostaId,
                NomeCliente = "Maria",
                TipoSeguro = "Auto",
                Valor = 2000m,
                Status = StatusProposta.EmAnalise
            };

            _propostaRepositoryMock
                .Setup(r => r.ObterPorIdAsync(propostaId))
                .ReturnsAsync(propostaMock);

            _propostaRepositoryMock
                .Setup(r => r.AtualizarAsync(propostaMock))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await service.AlterarStatusAsync(propostaId, StatusProposta.Aprovada);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(StatusProposta.Aprovada, resultado!.Status);

            _propostaRepositoryMock.Verify(r => r.AtualizarAsync(propostaMock), Times.Once);
            _rabbitMQServiceMock.Verify(r => r.EnviarProposta(It.Is<PropostaEnviadaEvent>(
                e => e.Id == propostaId &&
                     e.Status == StatusProposta.Aprovada.ToString()
            )), Times.Once);
        }
    }
}
