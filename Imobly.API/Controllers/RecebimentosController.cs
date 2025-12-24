using Imobly.Application.DTOs.Recebimentos;
using Imobly.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Imobly.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecebimentosController : ApiControllerBase
    {
        private readonly IRecebimentoService _recebimentoService;

        public RecebimentosController(IRecebimentoService recebimentoService)
        {
            _recebimentoService = recebimentoService;
        }

        /// <summary>
        /// Obtém recebimentos por contrato
        /// </summary>
        [HttpGet("contrato/{contratoId}")]
        [ProducesResponseType(typeof(IEnumerable<RecebimentoDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByContrato(Guid contratoId)
        {
            try
            {
                var recebimentos = await _recebimentoService.GetByContratoAsync(contratoId, UsuarioId);
                return Ok(recebimentos);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém recebimentos pendentes do usuário
        /// </summary>
        [HttpGet("pendentes")]
        [ProducesResponseType(typeof(IEnumerable<RecebimentoDto>), 200)]
        public async Task<IActionResult> GetPendentes()
        {
            try
            {
                var recebimentos = await _recebimentoService.GetPendentesByUsuarioAsync(UsuarioId);
                return Ok(recebimentos);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém recebimentos atrasados do usuário
        /// </summary>
        [HttpGet("atrasados")]
        [ProducesResponseType(typeof(IEnumerable<RecebimentoDto>), 200)]
        public async Task<IActionResult> GetAtrasados()
        {
            try
            {
                var recebimentos = await _recebimentoService.GetAtrasadosByUsuarioAsync(UsuarioId);
                return Ok(recebimentos);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Registra pagamento de um recebimento
        /// </summary>
        [HttpPut("{id}/pagar")]
        [ProducesResponseType(typeof(RecebimentoDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RegistrarPagamento(Guid id, [FromBody] RegistrarPagamentoDto dto)
        {
            try
            {
                var recebimento = await _recebimentoService.RegistrarPagamentoAsync(id, dto, UsuarioId);
                return Ok(recebimento);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gera recebimentos mensais para um contrato
        /// </summary>
        [HttpPost("gerar")]
        [ProducesResponseType(typeof(IEnumerable<RecebimentoDto>), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GerarRecebimentos([FromBody] GerarRecebimentosDto dto)
        {
            try
            {
                var recebimentos = await _recebimentoService.GerarRecebimentosAsync(dto, UsuarioId);
                return CreatedAtAction(nameof(GetByContrato), new { contratoId = dto.ContratoId }, recebimentos);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém total recebido no mês atual
        /// </summary>
        [HttpGet("total-mes")]
        [ProducesResponseType(typeof(decimal), 200)]
        public async Task<IActionResult> GetTotalMesAtual()
        {
            try
            {
                var hoje = DateTime.UtcNow;
                var total = await _recebimentoService.GetTotalRecebidoNoMesAsync(UsuarioId, hoje.Month, hoje.Year);
                return Ok(new { total = total });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém total recebido em um mês específico
        /// </summary>
        [HttpGet("total/{ano}/{mes}")]
        [ProducesResponseType(typeof(decimal), 200)]
        public async Task<IActionResult> GetTotalPorMes(int ano, int mes)
        {
            try
            {
                var total = await _recebimentoService.GetTotalRecebidoNoMesAsync(UsuarioId, mes, ano);
                return Ok(new { total = total });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}