using Imobly.Domain.Entities;
using Imobly.Domain.Interfaces;
using Imobly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Imobly.Infrastructure.Repositories
{
    public class MovimentacaoFinanceiraRepository : Repository<MovimentacaoFinanceira>, IMovimentacaoFinanceiraRepository
    {
        public MovimentacaoFinanceiraRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MovimentacaoFinanceira>> GetByImovelIdAsync(Guid imovelId)
        {
            return await _context.MovimentacoesFinanceiras
                .Where(m => m.ImovelId == imovelId)
                .OrderByDescending(m => m.Data)
                .ToListAsync();
        }

        public async Task<IEnumerable<MovimentacaoFinanceira>> GetByPeriodoAsync(Guid usuarioId, DateTime inicio, DateTime fim)
        {
            return await _context.MovimentacoesFinanceiras
                .Include(m => m.Imovel)
                .Where(m => m.Imovel.UsuarioId == usuarioId &&
                           m.Data >= inicio && m.Data <= fim)
                .OrderByDescending(m => m.Data)
                .ToListAsync();
        }

        public async Task<decimal> GetSaldoPeriodoAsync(Guid usuarioId, DateTime inicio, DateTime fim)
        {
            var movimentacoes = await _context.MovimentacoesFinanceiras
                .Include(m => m.Imovel)
                .Where(m => m.Imovel.UsuarioId == usuarioId &&
                           m.Data >= inicio && m.Data <= fim)
                .ToListAsync();

            var receitas = movimentacoes
                .Where(m => m.Tipo == Domain.Enums.TipoMovimentacao.Receita &&
                           m.Status == Domain.Enums.StatusMovimentacao.Pago)
                .Sum(m => m.Valor);

            var despesas = movimentacoes
                .Where(m => m.Tipo == Domain.Enums.TipoMovimentacao.Despesa &&
                           m.Status == Domain.Enums.StatusMovimentacao.Pago)
                .Sum(m => m.Valor);

            return receitas - despesas;
        }
    }
}