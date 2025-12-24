using Imobly.Domain.Entities;

namespace Imobly.Domain.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<IEnumerable<Usuario>> GetUsuariosComImoveisAsync();
    }
}