using Imobly.Domain.Entities;

namespace Imobly.Domain.Interfaces
{
    public interface IMovimentacaoFinanceiraRepository : IRepository<MovimentacaoFinanceira>
    {
        Task<IEnumerable<MovimentacaoFinanceira>> GetByImovelIdAsync(Guid imovelId);
        Task<IEnumerable<MovimentacaoFinanceira>> GetByPeriodoAsync(Guid usuarioId, DateTime inicio, DateTime fim);
        Task<decimal> GetSaldoPeriodoAsync(Guid usuarioId, DateTime inicio, DateTime fim);
    }
}