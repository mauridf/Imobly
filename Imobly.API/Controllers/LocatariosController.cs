using Imobly.Application.DTOs.Locatarios;
using Imobly.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Imobly.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocatariosController : ApiControllerBase
    {
        private readonly ILocatarioService _locatarioService;

        public LocatariosController(ILocatarioService locatarioService)
        {
            _locatarioService = locatarioService;
        }

        /// <summary>
        /// Obtém todos os locatários
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LocatarioDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var locatarios = await _locatarioService.GetAllAsync();
                return Ok(locatarios);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém um locatário pelo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LocatarioDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var locatario = await _locatarioService.GetByIdAsync(id);
                return OkOrNotFound(locatario);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Cria um novo locatário
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(LocatarioDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CriarLocatarioDto dto)
        {
            try
            {
                var locatario = await _locatarioService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = locatario.Id }, locatario);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Atualiza um locatário existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(LocatarioDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] AtualizarLocatarioDto dto)
        {
            try
            {
                var locatario = await _locatarioService.UpdateAsync(id, dto);
                return Ok(locatario);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Exclui um locatário
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _locatarioService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Busca locatários por termo
        /// </summary>
        [HttpGet("buscar")]
        [ProducesResponseType(typeof(IEnumerable<LocatarioDto>), 200)]
        public async Task<IActionResult> Search([FromQuery] string termo)
        {
            try
            {
                var locatarios = await _locatarioService.SearchAsync(termo);
                return Ok(locatarios);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Marca locatário como adimplente
        /// </summary>
        [HttpPut("{id}/adimplente")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> MarcarComoAdimplente(Guid id)
        {
            try
            {
                await _locatarioService.MarcarComoAdimplenteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Marca locatário como inadimplente
        /// </summary>
        [HttpPut("{id}/inadimplente")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> MarcarComoInadimplente(Guid id)
        {
            try
            {
                await _locatarioService.MarcarComoInadimplenteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}