using Imobly.Application.DTOs.Dashboard;
using Imobly.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Imobly.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ApiControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Obtém resumo do dashboard
        /// </summary>
        [HttpGet("resumo")]
        [ProducesResponseType(typeof(DashboardResumoDto), 200)]
        public async Task<IActionResult> GetResumo()
        {
            try
            {
                var resumo = await _dashboardService.GetResumoAsync(UsuarioId);
                return Ok(resumo);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém gráfico de receitas e despesas
        /// </summary>
        [HttpGet("grafico-receita-despesa")]
        [ProducesResponseType(typeof(IEnumerable<GraficoReceitaDespesaDto>), 200)]
        public async Task<IActionResult> GetGraficoReceitaDespesa([FromQuery] int meses = 6)
        {
            try
            {
                var grafico = await _dashboardService.GetGraficoReceitaDespesaAsync(UsuarioId, meses);
                return Ok(grafico);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém contratos próximos do vencimento
        /// </summary>
        [HttpGet("contratos-vencimento")]
        [ProducesResponseType(typeof(IEnumerable<ContratoProximoVencimentoDto>), 200)]
        public async Task<IActionResult> GetContratosProximosVencimento([FromQuery] int dias = 30)
        {
            try
            {
                var contratos = await _dashboardService.GetContratosProximosVencimentoAsync(UsuarioId, dias);
                return Ok(contratos);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém manutenções pendentes
        /// </summary>
        [HttpGet("manutencoes-pendentes")]
        [ProducesResponseType(typeof(IEnumerable<ManutencaoPendenteDto>), 200)]
        public async Task<IActionResult> GetManutencoesPendentes()
        {
            try
            {
                var manutencoes = await _dashboardService.GetManutencoesPendentesAsync(UsuarioId);
                return Ok(manutencoes);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém estatísticas detalhadas
        /// </summary>
        [HttpGet("estatisticas-detalhadas")]
        [ProducesResponseType(typeof(EstatisticasDetalhadasDto), 200)]
        public async Task<IActionResult> GetEstatisticasDetalhadas()
        {
            try
            {
                var estatisticas = await _dashboardService.GetEstatisticasDetalhadasAsync(UsuarioId);
                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}