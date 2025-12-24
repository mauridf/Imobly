using System.Diagnostics.Contracts;
using Imobly.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Imobly.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tabelas do sistema
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Imovel> Imoveis { get; set; }
        public DbSet<MidiaImovel> MidiasImoveis { get; set; }
        public DbSet<Locatario> Locatarios { get; set; }
        public DbSet<Contrato> Contratos { get; set; }
        public DbSet<Recebimento> Recebimentos { get; set; }
        public DbSet<MovimentacaoFinanceira> MovimentacoesFinanceiras { get; set; }
        public DbSet<Manutencao> Manutencoes { get; set; }
        public DbSet<Seguro> Seguros { get; set; }
        public DbSet<HistoricoReajuste> HistoricosReajuste { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Nome).IsRequired().HasMaxLength(150);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.Property(u => u.Telefone).HasMaxLength(20);

                // 1:N com Imovel
                entity.HasMany(u => u.Imoveis)
                      .WithOne(i => i.Usuario)
                      .HasForeignKey(i => i.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configurar Imovel
            modelBuilder.Entity<Imovel>(entity =>
            {
                entity.Property(i => i.Titulo).IsRequired().HasMaxLength(150);
                entity.Property(i => i.EnderecoLogradouro).HasMaxLength(200);
                entity.Property(i => i.EnderecoNumero).HasMaxLength(20);
                entity.Property(i => i.EnderecoComplemento).HasMaxLength(100);
                entity.Property(i => i.EnderecoBairro).HasMaxLength(100);
                entity.Property(i => i.EnderecoCidade).HasMaxLength(100);
                entity.Property(i => i.EnderecoEstado).HasMaxLength(2);
                entity.Property(i => i.EnderecoCEP).HasMaxLength(10);
                entity.Property(i => i.AreaM2).HasPrecision(10, 2);
                entity.Property(i => i.ValorAluguelSugerido).HasPrecision(10, 2);

                // 1:N com Contrato
                entity.HasMany(i => i.Contratos)
                      .WithOne(c => c.Imovel)
                      .HasForeignKey(c => c.ImovelId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configurar Locatario
            modelBuilder.Entity<Locatario>(entity =>
            {
                entity.HasIndex(l => l.CPF).IsUnique();
                entity.Property(l => l.Nome).IsRequired().HasMaxLength(150);
                entity.Property(l => l.CPF).IsRequired().HasMaxLength(14);
                entity.Property(l => l.RG).HasMaxLength(30);
            });

            // Configurar Contrato
            modelBuilder.Entity<Contrato>(entity =>
            {
                entity.Property(c => c.ValorAluguel).HasPrecision(10, 2);
                entity.Property(c => c.ValorSeguro).HasPrecision(10, 2);
                entity.Property(c => c.CaminhoDocumentoPDF).HasMaxLength(255);

                // 1:N com Recebimento
                entity.HasMany(c => c.Recebimentos)
                      .WithOne(r => r.Contrato)
                      .HasForeignKey(r => r.ContratoId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Restrição: Um imóvel só pode ter um contrato ativo por vez
                entity.HasIndex(c => new { c.ImovelId, c.Status })
                      .HasFilter("Status = 1") // Status = Ativo
                      .IsUnique(false);
            });

            // Configurar Recebimento
            modelBuilder.Entity<Recebimento>(entity =>
            {
                entity.Property(r => r.ValorPrevisto).HasPrecision(10, 2);
                entity.Property(r => r.ValorPago).HasPrecision(10, 2);
            });

            // Configurar valores padrão para CreatedAt
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType.IsSubclassOf(typeof(BaseEntity)))
                {
                    modelBuilder.Entity(entityType.Name)
                        .Property("CriadoEm")
                        .HasDefaultValueSql("NOW()");
                }
            }
        }
    }
}