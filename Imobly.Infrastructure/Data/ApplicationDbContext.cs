using System.Diagnostics.Contracts;
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
        //public DbSet<Usuario> Usuarios { get; set; }
        //public DbSet<Imovel> Imoveis { get; set; }
        //public DbSet<MidiaImovel> MidiasImoveis { get; set; }
        //public DbSet<Locatario> Locatarios { get; set; }
        //public DbSet<Contrato> Contratos { get; set; }
        //public DbSet<Recebimento> Recebimentos { get; set; }
        //public DbSet<MovimentacaoFinanceira> MovimentacoesFinanceiras { get; set; }
        //public DbSet<Manutencao> Manutencoes { get; set; }
        //public DbSet<Seguro> Seguros { get; set; }
        //public DbSet<HistoricoReajuste> HistoricosReajuste { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações das entidades serão feitas aqui
            // Usaremos Fluent API para configurações específicas
        }
    }
}