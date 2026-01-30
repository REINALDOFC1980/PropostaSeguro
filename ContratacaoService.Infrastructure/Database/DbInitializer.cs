using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ContratacaoService.Infrastructure.Database;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var db = scope.ServiceProvider
                      .GetRequiredService<ContratacaoDbContext>();

        await db.Database.MigrateAsync();
    }
}
