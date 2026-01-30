using Microsoft.EntityFrameworkCore;
using PropostaService.Infrastructure.Database;
using PropostaService.Domain.Entities;

public class IdempotencyRepository : IIdempotencyRepository
{
    private readonly PropostaDbContext _context;

    public IdempotencyRepository(PropostaDbContext context)
    {
        _context = context;
    }

    public async Task<IdempotencyKey?> ObterPorChaveAsync(string key)
    {
        return await _context.IdempotencyKeys.FirstOrDefaultAsync(x => x.Key == key);
    }

    public async Task AdicionarAsync(IdempotencyKey key)
    {
        _context.IdempotencyKeys.Add(key);
        await _context.SaveChangesAsync();
    }
}
