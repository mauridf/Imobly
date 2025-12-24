using Imobly.Application.DTOs.Reajustes;
using Imobly.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Imobly.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReajustesController : ApiControllerBase
    {
        private readonly IHistoricoReajusteService _reajusteService;

        public ReajustesController(IHistoricoReajusteService reajusteService)
        {
            _reajusteService = reajusteService;
        }

        /// <summary>
        /// Obtém histórico de reajustes por contrato
        /// </summary>
        [HttpGet("contrato/{contratoId}")]
        [ProducesResponseType(typeof(IEnumerable<HistoricoReajusteDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByContrato(Guid contratoId)
        {
            try
            {
                var historicos = await _reajusteService.GetByContratoAsync(contratoId, UsuarioId);
                return Ok(historicos);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém um histórico de reajuste pelo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(HistoricoReajusteDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var historico = await _reajusteService.GetByIdAsync(id, UsuarioId);
                return OkOrNotFound(historico);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém os últimos reajustes realizados
        /// </summary>
        [HttpGet("ultimos")]
        [ProducesResponseType(typeof(IEnumerable<HistoricoReajusteDto>), 200)]
        public async Task<IActionResult> GetUltimosReajustes([FromQuery] int quantidade = 10)
        {
            try
            {
                var historicos = await _reajusteService.GetUltimosReajustesAsync(UsuarioId, quantidade);
                return Ok(historicos);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Cria um novo histórico de reajuste
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(HistoricoReajusteDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CriarHistoricoReajusteDto dto)
        {
            try
            {
                var historico = await _reajusteService.CreateAsync(dto, UsuarioId);
                return CreatedAtAction(nameof(GetById), new { id = historico.Id }, historico);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Atualiza um histórico de reajuste existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(HistoricoReajusteDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] CriarHistoricoReajusteDto dto)
        {
            try
            {
                var historico = await _reajusteService.UpdateAsync(id, dto, UsuarioId);
                return Ok(historico);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Exclui um histórico de reajuste
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _reajusteService.DeleteAsync(id, UsuarioId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Calcula valor de reajuste baseado em índice
        /// </summary>
        [HttpPost("calcular")]
        [ProducesResponseType(typeof(decimal), 200)]
        public async Task<IActionResult> CalcularReajuste(
            [FromBody] CalcularReajusteRequest request)
        {
            try
            {
                var novoValor = await _reajusteService.CalcularReajusteAsync(
                    request.ValorAtual,
                    request.Indice,
                    request.Percentual);

                return Ok(new
                {
                    valorAtual = request.ValorAtual,
                    novoValor = novoValor,
                    aumento = novoValor - request.ValorAtual,
                    percentual = request.Percentual,
                    indice = request.Indice
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Sugere reajuste baseado em índice oficial
        /// </summary>
        [HttpGet("sugerir/{contratoId}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SugerirReajuste(Guid contratoId, [FromQuery] string indice = "IPCA")
        {
            try
            {
                var sugestao = await _reajusteService.SugerirReajusteAsync(contratoId, UsuarioId, indice);
                return Ok(sugestao);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }

    // DTO para request de cálculo de reajuste
    public class CalcularReajusteRequest
    {
        public decimal ValorAtual { get; set; }
        public string Indice { get; set; } = "IPCA";
        public decimal Percentual { get; set; }
    }
}