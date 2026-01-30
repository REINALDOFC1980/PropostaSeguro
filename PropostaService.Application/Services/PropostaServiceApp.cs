using Microsoft.Extensions.Logging;
using PropostaService.Application.Interfaces;
using PropostaService.Domain.Entities;

namespace PropostaService.Application.Services
{
    public class PropostaServiceApp
    {
        private readonly IPropostaRepository _repository;
        private readonly ILogger<PropostaServiceApp> _logger;
        private readonly IRabbitMQService _rabbitMQService;

        public PropostaServiceApp(
            IPropostaRepository repository,
            ILogger<PropostaServiceApp> logger,
            IRabbitMQService rabbitMQService)
        {
            _repository = repository;
            _logger = logger;
            _rabbitMQService = rabbitMQService;
        }

        //ADD ROOLBACK AQUI!
        public async Task<PropostaModel> CriarPropostaAsync(string nomeCliente, string tipoSeguro, decimal valor)
        {
            _logger.LogInformation("Iniciando criação de proposta para {Cliente}", nomeCliente);

            var proposta = new PropostaModel
            {
                Id = Guid.NewGuid(),
                NomeCliente = nomeCliente,
                TipoSeguro = tipoSeguro,
                Valor = valor,
                Status = StatusProposta.EmAnalise
            };

            // Salva no banco
            var propostaCriada = await _repository.AdicionarAsync(proposta);
            _logger.LogInformation("Proposta criada com Id {Id}", propostaCriada.Id);

            // Envia para RabbitMQ
            var evento = new PropostaEnviadaEvent
            {
                Id = propostaCriada.Id,
                NomeCliente = propostaCriada.NomeCliente,
                TipoSeguro = propostaCriada.TipoSeguro,
                Valor = propostaCriada.Valor,
                Status = propostaCriada.Status.ToString(),
                CriadoEm = propostaCriada.CriadoEm
            };

            try
            {
                _rabbitMQService.EnviarProposta(evento);
            }
            catch (Exception ex)
            {

                throw;
            }
        

            _logger.LogInformation("Proposta enviada para RabbitMQ com Id {Id}", propostaCriada.Id);

            return propostaCriada;
        }

        public async Task<List<PropostaModel>> ListarPropostasAsync()
        {
            return await _repository.ListarAsync();
        }

        public async Task<PropostaModel?> AlterarStatusAsync(Guid id, StatusProposta status)
        {
            var proposta = await _repository.ObterPorIdAsync(id);
            if (proposta == null) return null;

            proposta.Status = status;
            await _repository.AtualizarAsync(proposta);

            _logger.LogInformation("Status alterado para a proposta com Id {Id}", proposta.Id);
            return proposta;
        }
    }
}
