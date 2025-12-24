using Imobly.Domain.Entities;
using Imobly.Domain.Enums;
using Imobly.Domain.Interfaces;
using Imobly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Imobly.Infrastructure.Repositories
{
    public class ImovelRepository : Repository<Imovel>, IImovelRepository
    {
        public ImovelRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Imovel>> GetByUsuarioIdAsync(Guid usuarioId)
        {
            return await _context.Imoveis
                .Where(i => i.UsuarioId == usuarioId)
                .Include(i => i.Contratos)
                .ToListAsync();
        }

        public async Task<IEnumerable<Imovel>> GetByTipoAsync(TipoImovel tipo)
        {
            return await _context.Imoveis
                .Where(i => i.Tipo == tipo && i.Ativo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Imovel>> SearchAsync(string searchTerm, Guid? usuarioId = null)
        {
            var query = _context.Imoveis.AsQueryable();

            if (usuarioId.HasValue)
            {
                query = query.Where(i => i.UsuarioId == usuarioId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(i =>
                    i.Titulo.Contains(searchTerm) ||
                    i.Descricao.Contains(searchTerm) ||
                    i.EnderecoLogradouro.Contains(searchTerm) ||
                    i.EnderecoBairro.Contains(searchTerm) ||
                    i.EnderecoCidade.Contains(searchTerm));
            }

            return await query
                .Where(i => i.Ativo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Imovel>> GetComContratosAtivosAsync(Guid usuarioId)
        {
            return await _context.Imoveis
                .Where(i => i.UsuarioId == usuarioId)
                .Include(i => i.Contratos.Where(c => c.Status == StatusContrato.Ativo))
                .Where(i => i.Contratos.Any(c => c.Status == StatusContrato.Ativo))
                .ToListAsync();
        }

        public async Task<Imovel> GetWithDetailsAsync(Guid id)
        {
            return await _context.Imoveis
                .Include(i => i.Usuario)
                .Include(i => i.Contratos)
                .Include(i => i.Midias)
                .Include(i => i.Manutencoes)
                .Include(i => i.Seguros)
                .Include(i => i.MovimentacoesFinanceiras)
                .FirstOrDefaultAsync(i => i.Id == id);
        }
    }
}