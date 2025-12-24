using Imobly.Domain.Entities;
using Imobly.Domain.Enums;
using Imobly.Domain.Interfaces;
using Imobly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Imobly.Infrastructure.Repositories
{
    public class ContratoRepository : Repository<Contrato>, IContratoRepository
    {
        public ContratoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Contrato>> GetByImovelIdAsync(Guid imovelId)
        {
            return await _context.Contratos
                .Where(c => c.ImovelId == imovelId)
                .Include(c => c.Locatario)
                .OrderByDescending(c => c.DataInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contrato>> GetByLocatarioIdAsync(Guid locatarioId)
        {
            return await _context.Contratos
                .Where(c => c.LocatarioId == locatarioId)
                .Include(c => c.Imovel)
                .OrderByDescending(c => c.DataInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contrato>> GetAtivosByUsuarioIdAsync(Guid usuarioId)
        {
            return await _context.Contratos
                .Include(c => c.Imovel)
                .Include(c => c.Locatario)
                .Where(c => c.Imovel.UsuarioId == usuarioId && c.Status == StatusContrato.Ativo)
                .OrderBy(c => c.DataFim)
                .ToListAsync();
        }

        public async Task<Contrato> GetWithDetailsAsync(Guid id)
        {
            return await _context.Contratos
                .Include(c => c.Imovel)
                    .ThenInclude(i => i.Usuario)
                .Include(c => c.Locatario)
                .Include(c => c.Recebimentos)
                .Include(c => c.HistoricosReajuste)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> HasContratoAtivoAsync(Guid imovelId)
        {
            return await _context.Contratos
                .AnyAsync(c => c.ImovelId == imovelId && c.Status == StatusContrato.Ativo);
        }
    }
}