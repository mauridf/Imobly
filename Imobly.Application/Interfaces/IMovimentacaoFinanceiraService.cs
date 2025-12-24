using Imobly.Application.DTOs.Movimentacoes;

namespace Imobly.Application.Interfaces
{
    public interface IMovimentacaoFinanceiraService
    {
        Task<MovimentacaoFinanceiraDto> GetByIdAsync(Guid id, Guid usuarioId);
        Task<IEnumerable<MovimentacaoFinanceiraDto>> GetByImovelAsync(Guid imovelId, Guid usuarioId);
        Task<IEnumerable<MovimentacaoFinanceiraDto>> GetByPeriodoAsync(Guid usuarioId, DateTime inicio, DateTime fim);
        Task<IEnumerable<MovimentacaoFinanceiraDto>> GetByCategoriaAsync(string categoria, Guid usuarioId);
        Task<MovimentacaoFinanceiraDto> CreateAsync(CriarMovimentacaoFinanceiraDto dto, Guid usuarioId);
        Task<MovimentacaoFinanceiraDto> UpdateAsync(Guid id, AtualizarMovimentacaoFinanceiraDto dto, Guid usuarioId);
        Task<bool> DeleteAsync(Guid id, Guid usuarioId);
        Task<bool> RegistrarPagamentoAsync(Guid id, Guid usuarioId);
        Task<decimal> GetSaldoPeriodoAsync(Guid usuarioId, DateTime inicio, DateTime fim);
        Task<object> GerarRelatorioFinanceiroAsync(Guid usuarioId, int ano);
    }
}