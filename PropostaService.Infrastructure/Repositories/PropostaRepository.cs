using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PropostaService.Application.Interfaces;
using PropostaService.Domain.Entities;
using PropostaService.Infrastructure.Database;

namespace PropostaService.Infrastructure.Repositories
{
    public class PropostaRepository : IPropostaRepository
    {
        private readonly PropostaDbContext _context;

        public PropostaRepository(PropostaDbContext context)
        {
            _context = context;
        }

        // ===========================
        // Transação genérica
        // ===========================
        public async Task<ITransaction> BeginTransactionAsync()
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            return new EfTransaction(transaction);
        }

        private class EfTransaction : ITransaction
        {
            private readonly IDbContextTransaction _transaction;
            public EfTransaction(IDbContextTransaction transaction)
            {
                _transaction = transaction;
            }

            public async Task CommitAsync() => await _transaction.CommitAsync();
            public async Task RollbackAsync() => await _transaction.RollbackAsync();
            public async ValueTask DisposeAsync() => await _transaction.DisposeAsync();
        }

        // ===========================
        // CRUD
        // ===========================
        public async Task<PropostaModel> AdicionarAsync(PropostaModel proposta)
        {
            _context.Propostas.Add(proposta);
            await _context.SaveChangesAsync();
            return proposta;
        }

        public async Task<List<PropostaModel>> ListarAsync()
        {
            return await _context.Propostas.AsNoTracking().ToListAsync();
        }

        public async Task<PropostaModel?> ObterPorIdAsync(Guid id)
        {
            return await _context.Propostas.FindAsync(id);
        }

        public async Task AtualizarAsync(PropostaModel proposta)
        {
            _context.Propostas.Update(proposta);
            await _context.SaveChangesAsync();
        }
    }
}
