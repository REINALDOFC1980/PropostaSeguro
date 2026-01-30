using Microsoft.EntityFrameworkCore;
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

        public async Task<PropostaModel> AdicionarAsync(PropostaModel PropostaModel)
        {
            _context.Propostas.Add(PropostaModel);
            await _context.SaveChangesAsync();
            return PropostaModel;
        }

        public async Task<List<PropostaModel>> ListarAsync()
        {
            return await _context.Propostas.ToListAsync();
        }

        public async Task<PropostaModel?> ObterPorIdAsync(Guid id)
        {
            return await _context.Propostas.FindAsync(id);
        }

        public async Task AtualizarAsync(PropostaModel PropostaModel)
        {
            _context.Propostas.Update(PropostaModel);
            await _context.SaveChangesAsync();
        }
    }
}
