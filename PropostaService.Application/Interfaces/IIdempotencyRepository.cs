using PropostaService.Domain.Entities;

public interface IIdempotencyRepository
{
    Task<IdempotencyKey?> ObterPorChaveAsync(string key);
    Task AdicionarAsync(IdempotencyKey key);
}
