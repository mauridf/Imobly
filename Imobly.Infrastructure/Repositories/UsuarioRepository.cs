using Imobly.Domain.Entities;
using Imobly.Domain.Interfaces;
using Imobly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Imobly.Infrastructure.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosComImoveisAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Imoveis)
                .Where(u => u.Imoveis.Any())
                .ToListAsync();
        }
    }
}