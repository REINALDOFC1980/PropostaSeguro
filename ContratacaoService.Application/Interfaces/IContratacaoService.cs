using ContratacaoService.Domain.Entities;

namespace ContratacaoService.Application.Interfaces
{
    public interface IContratacaoService
    {
        Task CriarContratacaoAsync(PropostaEnviadaEvent proposta);
        Task<List<ContratacaoModel>> ListarAsync();
    }
}
