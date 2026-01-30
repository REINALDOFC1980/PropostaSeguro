using Microsoft.EntityFrameworkCore;
using ContratacaoService.Domain.Entities;

namespace ContratacaoService.Infrastructure.Database
{
    public class ContratacaoDbContext : DbContext
    {
        public ContratacaoDbContext(DbContextOptions<ContratacaoDbContext> options) : base(options) { }

        public DbSet<ContratacaoModel> Contratacoes => Set<ContratacaoModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContratacaoModel>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.PropostaId)
                      .IsRequired();

                entity.Property(c => c.NomeCliente)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(c => c.TipoSeguro)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(c => c.Status)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(c => c.Valor)
                      .HasColumnType("decimal(18,2)");

                entity.Property(c => c.CriadoEm)
                      .HasDefaultValueSql("GETUTCDATE()");
            });
        }

    }
}
