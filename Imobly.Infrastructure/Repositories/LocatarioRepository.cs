using Imobly.Domain.Entities;
using Imobly.Domain.Interfaces;
using Imobly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Imobly.Infrastructure.Repositories
{
    public class LocatarioRepository : Repository<Locatario>, ILocatarioRepository
    {
        public LocatarioRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Locatario> GetByCpfAsync(string cpf)
        {
            return await _context.Locatarios
                .FirstOrDefaultAsync(l => l.CPF == cpf);
        }

        public async Task<IEnumerable<Locatario>> SearchAsync(string searchTerm)
        {
            return await _context.Locatarios
                .Where(l =>
                    l.Nome.Contains(searchTerm) ||
                    l.CPF.Contains(searchTerm) ||
                    l.Email.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<bool> CpfExistsAsync(string cpf)
        {
            return await _context.Locatarios
                .AnyAsync(l => l.CPF == cpf);
        }
    }
}