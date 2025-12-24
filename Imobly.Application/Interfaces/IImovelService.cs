using Imobly.Application.DTOs.Imoveis;

namespace Imobly.Application.Interfaces
{
    public interface IImovelService
    {
        Task<ImovelDto> GetByIdAsync(Guid id, Guid usuarioId);
        Task<IEnumerable<ImovelDto>> GetAllByUsuarioAsync(Guid usuarioId);
        Task<ImovelDto> CreateAsync(CriarImovelDto dto, Guid usuarioId);
        Task<ImovelDto> UpdateAsync(Guid id, AtualizarImovelDto dto, Guid usuarioId);
        Task<bool> DeleteAsync(Guid id, Guid usuarioId);
        Task<bool> AtivarAsync(Guid id, Guid usuarioId);
        Task<bool> DesativarAsync(Guid id, Guid usuarioId);
        Task<IEnumerable<ImovelDto>> SearchAsync(string searchTerm, Guid usuarioId);
    }
}