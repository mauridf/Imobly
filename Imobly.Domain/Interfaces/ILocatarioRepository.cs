using Imobly.Domain.Entities;

namespace Imobly.Domain.Interfaces
{
    public interface ILocatarioRepository : IRepository<Locatario>
    {
        Task<Locatario> GetByCpfAsync(string cpf);
        Task<IEnumerable<Locatario>> SearchAsync(string searchTerm);
        Task<bool> CpfExistsAsync(string cpf);
    }
}