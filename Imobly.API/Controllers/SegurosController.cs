using Imobly.Application.DTOs.Seguros;
using Imobly.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Imobly.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SegurosController : ApiControllerBase
    {
        private readonly ISeguroService _seguroService;

        public SegurosController(ISeguroService seguroService)
        {
            _seguroService = seguroService;
        }

        /// <summary>
        /// Obtém seguros por imóvel
        /// </summary>
        [HttpGet("imovel/{imovelId}")]
        [ProducesResponseType(typeof(IEnumerable<SeguroDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByImovel(Guid imovelId)
        {
            try
            {
                var seguros = await _seguroService.GetByImovelAsync(imovelId, UsuarioId);
                return Ok(seguros);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém um seguro pelo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SeguroDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var seguro = await _seguroService.GetByIdAsync(id, UsuarioId);
                return OkOrNotFound(seguro);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém seguros vencendo nos próximos 30 dias
        /// </summary>
        [HttpGet("vencendo-proximos-30-dias")]
        [ProducesResponseType(typeof(IEnumerable<SeguroDto>), 200)]
        public async Task<IActionResult> GetVencendoProximos30Dias()
        {
            try
            {
                var seguros = await _seguroService.GetVencendoProximos30DiasAsync(UsuarioId);
                return Ok(seguros);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Cria um novo seguro
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(SeguroDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CriarSeguroDto dto)
        {
            try
            {
                var seguro = await _seguroService.CreateAsync(dto, UsuarioId);
                return CreatedAtAction(nameof(GetById), new { id = seguro.Id }, seguro);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Atualiza um seguro existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SeguroDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] AtualizarSeguroDto dto)
        {
            try
            {
                var seguro = await _seguroService.UpdateAsync(id, dto, UsuarioId);
                return Ok(seguro);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Exclui um seguro
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _seguroService.DeleteAsync(id, UsuarioId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Busca seguros por seguradora ou apólice
        /// </summary>
        [HttpGet("buscar")]
        [ProducesResponseType(typeof(IEnumerable<SeguroDto>), 200)]
        public async Task<IActionResult> Search(
            [FromQuery] string? seguradora = null,
            [FromQuery] string? apolice = null)
        {
            try
            {
                var seguros = await _seguroService.SearchAsync(seguradora, apolice, UsuarioId);
                return Ok(seguros);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}