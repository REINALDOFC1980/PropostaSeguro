using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PropostaService.Infrastructure.Database;

namespace PropostaService.Infrastructure.Database
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PropostaDbContext>();

            await context.Database.MigrateAsync();
        }
    }
}
