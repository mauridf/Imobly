using Imobly.Application.DTOs.Locatarios;

namespace Imobly.Application.Interfaces
{
    public interface ILocatarioService
    {
        Task<LocatarioDto> GetByIdAsync(Guid id);
        Task<IEnumerable<LocatarioDto>> GetAllAsync();
        Task<LocatarioDto> CreateAsync(CriarLocatarioDto dto);
        Task<LocatarioDto> UpdateAsync(Guid id, AtualizarLocatarioDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<LocatarioDto>> SearchAsync(string searchTerm);
        Task<bool> MarcarComoAdimplenteAsync(Guid id);
        Task<bool> MarcarComoInadimplenteAsync(Guid id);
    }
}