using Imobly.Application.DTOs.Recebimentos;

namespace Imobly.Application.Interfaces
{
    public interface IRecebimentoService
    {
        Task<IEnumerable<RecebimentoDto>> GetByContratoAsync(Guid contratoId, Guid usuarioId);
        Task<RecebimentoDto> RegistrarPagamentoAsync(Guid id, RegistrarPagamentoDto dto, Guid usuarioId);
        Task<IEnumerable<RecebimentoDto>> GerarRecebimentosAsync(GerarRecebimentosDto dto, Guid usuarioId);
        Task<IEnumerable<RecebimentoDto>> GetPendentesByUsuarioAsync(Guid usuarioId);
        Task<IEnumerable<RecebimentoDto>> GetAtrasadosByUsuarioAsync(Guid usuarioId);
        Task<decimal> GetTotalRecebidoNoMesAsync(Guid usuarioId, int mes, int ano);
    }
}