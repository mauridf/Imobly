using Imobly.Application.DTOs.Contratos;

namespace Imobly.Application.Interfaces
{
    public interface IContratoService
    {
        Task<ContratoDto> GetByIdAsync(Guid id, Guid usuarioId);
        Task<IEnumerable<ContratoDto>> GetByUsuarioAsync(Guid usuarioId);
        Task<ContratoDto> CreateAsync(CriarContratoDto dto, Guid usuarioId);
        Task<ContratoDto> UpdateAsync(Guid id, AtualizarContratoDto dto, Guid usuarioId);
        Task<bool> DeleteAsync(Guid id, Guid usuarioId);
        Task<bool> EncerrarAsync(Guid id, Guid usuarioId);
        Task<bool> SuspenderAsync(Guid id, Guid usuarioId);
        Task<bool> ReativarAsync(Guid id, Guid usuarioId);
        Task<ContratoComDetalhesDto> GetWithDetailsAsync(Guid id, Guid usuarioId);
    }
}