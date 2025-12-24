using Imobly.Application.DTOs.Seguros;

namespace Imobly.Application.Interfaces
{
    public interface ISeguroService
    {
        Task<SeguroDto> GetByIdAsync(Guid id, Guid usuarioId);
        Task<IEnumerable<SeguroDto>> GetByImovelAsync(Guid imovelId, Guid usuarioId);
        Task<IEnumerable<SeguroDto>> GetVencendoProximos30DiasAsync(Guid usuarioId);
        Task<SeguroDto> CreateAsync(CriarSeguroDto dto, Guid usuarioId);
        Task<SeguroDto> UpdateAsync(Guid id, AtualizarSeguroDto dto, Guid usuarioId);
        Task<bool> DeleteAsync(Guid id, Guid usuarioId);
        Task<IEnumerable<SeguroDto>> SearchAsync(string seguradora, string apolice, Guid usuarioId);
    }
}