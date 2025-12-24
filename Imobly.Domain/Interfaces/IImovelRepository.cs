using Imobly.Domain.Entities;
using Imobly.Domain.Enums;

namespace Imobly.Domain.Interfaces
{
    public interface IImovelRepository : IRepository<Imovel>
    {
        Task<IEnumerable<Imovel>> GetByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<Imovel>> GetByTipoAsync(TipoImovel tipo);
        Task<IEnumerable<Imovel>> SearchAsync(string searchTerm, Guid? usuarioId = null);
        Task<IEnumerable<Imovel>> GetComContratosAtivosAsync(Guid usuarioId);
        Task<Imovel> GetWithDetailsAsync(Guid id);
    }
}