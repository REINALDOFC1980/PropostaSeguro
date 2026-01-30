

using PropostaService.Domain.Entities;


namespace PropostaService.Application.Interfaces
{
    public interface IPropostaRepository
    {
        Task<PropostaModel> AdicionarAsync(PropostaModel proposta);
        Task<List<PropostaModel>> ListarAsync();
        Task<PropostaModel?> ObterPorIdAsync(Guid id);
        Task AtualizarAsync(PropostaModel proposta);
    }
}
