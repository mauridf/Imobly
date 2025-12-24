using Imobly.Domain.Entities;
using Imobly.Domain.Enums;
using Imobly.Domain.Interfaces;
using Imobly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Imobly.Infrastructure.Repositories
{
    public class RecebimentoRepository : Repository<Recebimento>, IRecebimentoRepository
    {
        public RecebimentoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Recebimento>> GetByContratoIdAsync(Guid contratoId)
        {
            return await _context.Recebimentos
                .Where(r => r.ContratoId == contratoId)
                .OrderBy(r => r.Competencia)
                .ToListAsync();
        }

        public async Task<IEnumerable<Recebimento>> GetPendentesByUsuarioIdAsync(Guid usuarioId)
        {
            return await _context.Recebimentos
                .Include(r => r.Contrato)
                    .ThenInclude(c => c.Imovel)
                .Where(r => r.Contrato.Imovel.UsuarioId == usuarioId &&
                           r.Status == StatusRecebimento.Aguardando)
                .OrderBy(r => r.Competencia)
                .ToListAsync();
        }

        public async Task<IEnumerable<Recebimento>> GetAtrasadosByUsuarioIdAsync(Guid usuarioId)
        {
            var hoje = DateTime.UtcNow;

            return await _context.Recebimentos
                .Include(r => r.Contrato)
                    .ThenInclude(c => c.Imovel)
                .Where(r => r.Contrato.Imovel.UsuarioId == usuarioId &&
                           r.Status == StatusRecebimento.Aguardando &&
                           r.Competencia < hoje)
                .OrderBy(r => r.Competencia)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRecebidoNoMesAsync(Guid usuarioId, int mes, int ano)
        {
            var inicioMes = new DateTime(ano, mes, 1);
            var fimMes = inicioMes.AddMonths(1).AddDays(-1);

            return await _context.Recebimentos
                .Include(r => r.Contrato)
                    .ThenInclude(c => c.Imovel)
                .Where(r => r.Contrato.Imovel.UsuarioId == usuarioId &&
                           r.Status == StatusRecebimento.Pago &&
                           r.DataPagamento >= inicioMes &&
                           r.DataPagamento <= fimMes)
                .SumAsync(r => r.ValorPago);
        }
    }
}