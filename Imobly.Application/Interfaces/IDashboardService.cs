using Imobly.Application.DTOs.Dashboard;

namespace Imobly.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardResumoDto> GetResumoAsync(Guid usuarioId);
        Task<IEnumerable<GraficoReceitaDespesaDto>> GetGraficoReceitaDespesaAsync(Guid usuarioId, int meses = 6);
        Task<IEnumerable<ContratoProximoVencimentoDto>> GetContratosProximosVencimentoAsync(Guid usuarioId, int dias = 30);
        Task<IEnumerable<ManutencaoPendenteDto>> GetManutencoesPendentesAsync(Guid usuarioId);
        Task<EstatisticasDetalhadasDto> GetEstatisticasDetalhadasAsync(Guid usuarioId);
    }
}