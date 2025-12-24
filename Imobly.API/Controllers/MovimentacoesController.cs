using Imobly.Application.DTOs.Movimentacoes;
using Imobly.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Imobly.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimentacoesController : ApiControllerBase
    {
        private readonly IMovimentacaoFinanceiraService _movimentacaoService;

        public MovimentacoesController(IMovimentacaoFinanceiraService movimentacaoService)
        {
            _movimentacaoService = movimentacaoService;
        }

        /// <summary>
        /// Obtém movimentações por imóvel
        /// </summary>
        [HttpGet("imovel/{imovelId}")]
        [ProducesResponseType(typeof(IEnumerable<MovimentacaoFinanceiraDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByImovel(Guid imovelId)
        {
            try
            {
                var movimentacoes = await _movimentacaoService.GetByImovelAsync(imovelId, UsuarioId);
                return Ok(movimentacoes);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém movimentações por período
        /// </summary>
        [HttpGet("periodo")]
        [ProducesResponseType(typeof(IEnumerable<MovimentacaoFinanceiraDto>), 200)]
        public async Task<IActionResult> GetByPeriodo(
            [FromQuery] DateTime inicio,
            [FromQuery] DateTime fim)
        {
            try
            {
                var movimentacoes = await _movimentacaoService.GetByPeriodoAsync(UsuarioId, inicio, fim);
                return Ok(movimentacoes);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém movimentações por categoria
        /// </summary>
        [HttpGet("categoria/{categoria}")]
        [ProducesResponseType(typeof(IEnumerable<MovimentacaoFinanceiraDto>), 200)]
        public async Task<IActionResult> GetByCategoria(string categoria)
        {
            try
            {
                var movimentacoes = await _movimentacaoService.GetByCategoriaAsync(categoria, UsuarioId);
                return Ok(movimentacoes);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém uma movimentação pelo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MovimentacaoFinanceiraDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var movimentacao = await _movimentacaoService.GetByIdAsync(id, UsuarioId);
                return OkOrNotFound(movimentacao);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Cria uma nova movimentação financeira
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(MovimentacaoFinanceiraDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CriarMovimentacaoFinanceiraDto dto)
        {
            try
            {
                var movimentacao = await _movimentacaoService.CreateAsync(dto, UsuarioId);
                return CreatedAtAction(nameof(GetById), new { id = movimentacao.Id }, movimentacao);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Atualiza uma movimentação existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MovimentacaoFinanceiraDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] AtualizarMovimentacaoFinanceiraDto dto)
        {
            try
            {
                var movimentacao = await _movimentacaoService.UpdateAsync(id, dto, UsuarioId);
                return Ok(movimentacao);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Exclui uma movimentação
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _movimentacaoService.DeleteAsync(id, UsuarioId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Registra pagamento de uma movimentação
        /// </summary>
        [HttpPut("{id}/pagar")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RegistrarPagamento(Guid id)
        {
            try
            {
                await _movimentacaoService.RegistrarPagamentoAsync(id, UsuarioId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém saldo em um período
        /// </summary>
        [HttpGet("saldo-periodo")]
        [ProducesResponseType(typeof(decimal), 200)]
        public async Task<IActionResult> GetSaldoPeriodo(
            [FromQuery] DateTime inicio,
            [FromQuery] DateTime fim)
        {
            try
            {
                var saldo = await _movimentacaoService.GetSaldoPeriodoAsync(UsuarioId, inicio, fim);
                return Ok(new { saldo = saldo });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gera relatório financeiro anual
        /// </summary>
        [HttpGet("relatorio/{ano}")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GerarRelatorioFinanceiro(int ano)
        {
            try
            {
                var relatorio = await _movimentacaoService.GerarRelatorioFinanceiroAsync(UsuarioId, ano);
                return Ok(relatorio);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}