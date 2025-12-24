using Imobly.Application.DTOs.Contratos;
using Imobly.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Imobly.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContratosController : ApiControllerBase
    {
        private readonly IContratoService _contratoService;

        public ContratosController(IContratoService contratoService)
        {
            _contratoService = contratoService;
        }

        /// <summary>
        /// Obtém todos os contratos do usuário
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ContratoDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var contratos = await _contratoService.GetByUsuarioAsync(UsuarioId);
                return Ok(contratos);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém um contrato pelo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ContratoDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var contrato = await _contratoService.GetByIdAsync(id, UsuarioId);
                return OkOrNotFound(contrato);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém detalhes completos de um contrato
        /// </summary>
        [HttpGet("{id}/detalhes")]
        [ProducesResponseType(typeof(ContratoComDetalhesDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetDetalhes(Guid id)
        {
            try
            {
                var contrato = await _contratoService.GetWithDetailsAsync(id, UsuarioId);
                return OkOrNotFound(contrato);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Cria um novo contrato
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ContratoDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CriarContratoDto dto)
        {
            try
            {
                var contrato = await _contratoService.CreateAsync(dto, UsuarioId);
                return CreatedAtAction(nameof(GetById), new { id = contrato.Id }, contrato);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Atualiza um contrato existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ContratoDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] AtualizarContratoDto dto)
        {
            try
            {
                var contrato = await _contratoService.UpdateAsync(id, dto, UsuarioId);
                return Ok(contrato);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Exclui um contrato
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _contratoService.DeleteAsync(id, UsuarioId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Encerra um contrato
        /// </summary>
        [HttpPut("{id}/encerrar")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Encerrar(Guid id)
        {
            try
            {
                await _contratoService.EncerrarAsync(id, UsuarioId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Suspende um contrato
        /// </summary>
        [HttpPut("{id}/suspender")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Suspender(Guid id)
        {
            try
            {
                await _contratoService.SuspenderAsync(id, UsuarioId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Reativa um contrato
        /// </summary>
        [HttpPut("{id}/reativar")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Reativar(Guid id)
        {
            try
            {
                await _contratoService.ReativarAsync(id, UsuarioId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}