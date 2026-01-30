using Microsoft.Extensions.Logging;
using PropostaService.Application.Interfaces;
using PropostaService.Domain.Entities;

namespace PropostaService.Application.Services
{
    public class PropostaServiceApp
    {
        private readonly IPropostaRepository _repository;
        private readonly IIdempotencyRepository _idempotencyRepository;
        private readonly ILogger<PropostaServiceApp> _logger;
        private readonly IRabbitMQService _rabbitMQService;

        public PropostaServiceApp(
            IPropostaRepository repository,
            IIdempotencyRepository idempotencyRepository,
            ILogger<PropostaServiceApp> logger,
            IRabbitMQService rabbitMQService)
        {
            _repository = repository;
            _idempotencyRepository = idempotencyRepository;
            _logger = logger;
            _rabbitMQService = rabbitMQService;
        }

        public async Task<PropostaModel> CriarPropostaAsync(string nomeCliente, string tipoSeguro, decimal valor, string? idempotencyKey = null)
        {
            // Verifica se já existe uma proposta para essa idempotencyKey
            if (!string.IsNullOrEmpty(idempotencyKey))
            {
                var existente = await _idempotencyRepository.ObterPorChaveAsync(idempotencyKey);
                if (existente != null)
                {
                    var propostaExistente = await _repository.ObterPorIdAsync(existente.PropostaId);
                    _logger.LogInformation("Retornando proposta existente para idempotencyKey {Key}", idempotencyKey);
                    return propostaExistente!;
                }
            }

            await using var transaction = await _repository.BeginTransactionAsync();

            try
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

                // Salva IdempotencyKey
                if (!string.IsNullOrEmpty(idempotencyKey))
                {
                    await _idempotencyRepository.AdicionarAsync(new IdempotencyKey
                    {
                        Key = idempotencyKey,
                        PropostaId = propostaCriada.Id
                    });
                }

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

                _rabbitMQService.EnviarProposta(evento);
                _logger.LogInformation("Proposta enviada para RabbitMQ com Id {Id}", propostaCriada.Id);

                // Confirma transação
                await transaction.CommitAsync();

                return propostaCriada;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao criar/enviar proposta para {Cliente}", nomeCliente);
                throw; // ExceptionMiddleware irá capturar e retornar JSON padronizado
            }
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
