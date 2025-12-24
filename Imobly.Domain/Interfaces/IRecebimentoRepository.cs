using Imobly.Domain.Entities;

namespace Imobly.Domain.Interfaces
{
    public interface IRecebimentoRepository : IRepository<Recebimento>
    {
        Task<IEnumerable<Recebimento>> GetByContratoIdAsync(Guid contratoId);
        Task<IEnumerable<Recebimento>> GetPendentesByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<Recebimento>> GetAtrasadosByUsuarioIdAsync(Guid usuarioId);
        Task<decimal> GetTotalRecebidoNoMesAsync(Guid usuarioId, int mes, int ano);
    }
}