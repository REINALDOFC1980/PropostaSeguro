using Microsoft.EntityFrameworkCore;
using PropostaService.Domain.Entities;

namespace PropostaService.Infrastructure.Database
{
    public class PropostaDbContext : DbContext
    {
        public PropostaDbContext(DbContextOptions<PropostaDbContext> options) : base(options) { }

        public DbSet<PropostaModel> Propostas => Set<PropostaModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PropostaModel>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.NomeCliente)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(p => p.TipoSeguro)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.Valor)
                      .HasColumnType("decimal(18,2)");

                // Enum como string + default
                entity.Property(p => p.Status)
                      .HasConversion<string>()
                      .HasDefaultValue(StatusProposta.EmAnalise);

                // CriadoEm default GETUTCDATE()
                entity.Property(p => p.CriadoEm)
                      .HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
