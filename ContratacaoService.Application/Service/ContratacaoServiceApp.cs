using ContratacaoService.Application.Interfaces;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Infrastructure.Database;
using Microsoft.Extensions.Logging;

namespace ContratacaoService.Application.Services;

public class ContratacaoServiceApp : IContratacaoService
{
    private readonly ILogger<ContratacaoServiceApp> _logger;
    private readonly ContratacaoDbContext _context;

    public ContratacaoServiceApp(ILogger<ContratacaoServiceApp> logger, ContratacaoDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public Task<List<ContratacaoModel>> ListarAsync()
    {
        throw new NotImplementedException();
    }

    public async Task CriarContratacaoAsync(PropostaEnviadaEvent proposta)
    {
        _logger.LogInformation("Processando proposta {PropostaId}", proposta.Id);

        var contrato = new ContratacaoModel
        {
            Id = Guid.NewGuid(),
            PropostaId = proposta.Id,
            NomeCliente = proposta.NomeCliente,
            TipoSeguro = proposta.TipoSeguro,
            Valor = proposta.Valor,
            Status = proposta.Status,
            CriadoEm = DateTime.UtcNow
        };

        _context.Contratacoes.Add(contrato);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Contrato criado com Id {ContratoId}", contrato.Id);

       
    }
}
