using Imobly.Domain.Entities;

namespace Imobly.Domain.Interfaces
{
    public interface IContratoRepository : IRepository<Contrato>
    {
        Task<IEnumerable<Contrato>> GetByImovelIdAsync(Guid imovelId);
        Task<IEnumerable<Contrato>> GetByLocatarioIdAsync(Guid locatarioId);
        Task<IEnumerable<Contrato>> GetAtivosByUsuarioIdAsync(Guid usuarioId);
        Task<Contrato> GetWithDetailsAsync(Guid id);
        Task<bool> HasContratoAtivoAsync(Guid imovelId);
    }
}