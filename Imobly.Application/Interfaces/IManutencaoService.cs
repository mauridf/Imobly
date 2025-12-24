using Imobly.Application.DTOs.Manutencoes;

namespace Imobly.Application.Interfaces
{
    public interface IManutencaoService
    {
        Task<ManutencaoDto> GetByIdAsync(Guid id, Guid usuarioId);
        Task<IEnumerable<ManutencaoDto>> GetByImovelAsync(Guid imovelId, Guid usuarioId);
        Task<ManutencaoDto> CreateAsync(CriarManutencaoDto dto, Guid usuarioId);
        Task<ManutencaoDto> UpdateAsync(Guid id, AtualizarManutencaoDto dto, Guid usuarioId);
        Task<bool> DeleteAsync(Guid id, Guid usuarioId);
        Task<bool> MarcarComoFeitaAsync(Guid id, Guid usuarioId);
    }
}