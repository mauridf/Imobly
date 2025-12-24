using Imobly.Application.DTOs.Reajustes;

namespace Imobly.Application.Interfaces
{
    public interface IHistoricoReajusteService
    {
        Task<HistoricoReajusteDto> GetByIdAsync(Guid id, Guid usuarioId);
        Task<IEnumerable<HistoricoReajusteDto>> GetByContratoAsync(Guid contratoId, Guid usuarioId);
        Task<HistoricoReajusteDto> CreateAsync(CriarHistoricoReajusteDto dto, Guid usuarioId);
        Task<HistoricoReajusteDto> UpdateAsync(Guid id, CriarHistoricoReajusteDto dto, Guid usuarioId);
        Task<bool> DeleteAsync(Guid id, Guid usuarioId);
        Task<decimal> CalcularReajusteAsync(decimal valorAtual, string indice, decimal percentual);
        Task<IEnumerable<HistoricoReajusteDto>> GetUltimosReajustesAsync(Guid usuarioId, int quantidade = 10);
    }
}