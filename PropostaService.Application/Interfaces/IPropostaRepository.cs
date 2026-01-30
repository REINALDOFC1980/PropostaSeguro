using PropostaService.Domain.Entities;

namespace PropostaService.Application.Interfaces
{
    public interface IPropostaRepository
    {
        Task<PropostaModel> AdicionarAsync(PropostaModel proposta);
        Task AtualizarAsync(PropostaModel proposta);
        Task<PropostaModel?> ObterPorIdAsync(Guid id);
        Task<List<PropostaModel>> ListarAsync();

        // Transação genérica, implementada em Infrastructure
        Task<ITransaction> BeginTransactionAsync();
    }

    // Interface genérica de transação para desacoplar do EF
    public interface ITransaction : IAsyncDisposable
    {
        Task CommitAsync();
        Task RollbackAsync();
    }
}
