using Imobly.Domain.Entities;

namespace Imobly.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUsuarioRepository Usuarios { get; }
        IImovelRepository Imoveis { get; }
        ILocatarioRepository Locatarios { get; }
        IContratoRepository Contratos { get; }
        IRecebimentoRepository Recebimentos { get; }
        IMovimentacaoFinanceiraRepository MovimentacoesFinanceiras { get; }
        IRepository<Manutencao> Manutencoes { get; }
        IRepository<Seguro> Seguros { get; }
        IRepository<HistoricoReajuste> HistoricosReajuste { get; }
        IRepository<MidiaImovel> MidiasImoveis { get; }

        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}