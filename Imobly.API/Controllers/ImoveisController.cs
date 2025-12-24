using Imobly.Application.DTOs.Imoveis;
using Imobly.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Imobly.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImoveisController : ApiControllerBase
    {
        private readonly IImovelService _imovelService;

        public ImoveisController(IImovelService imovelService)
        {
            _imovelService = imovelService;
        }

        /// <summary>
        /// Obtém todos os imóveis do usuário
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ImovelDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var imoveis = await _imovelService.GetAllByUsuarioAsync(UsuarioId);
                return Ok(imoveis);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém um imóvel específico pelo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ImovelDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var imovel = await _imovelService.GetByIdAsync(id, UsuarioId);
                return OkOrNotFound(imovel);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Cria um novo imóvel
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ImovelDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CriarImovelDto dto)
        {
            try
            {
                var imovel = await _imovelService.CreateAsync(dto, UsuarioId);
                return CreatedAtAction(nameof(GetById), new { id = imovel.Id }, imovel);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Atualiza um imóvel existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ImovelDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] AtualizarImovelDto dto)
        {
            try
            {
                var imovel = await _imovelService.UpdateAsync(id, dto, UsuarioId);
                return Ok(imovel);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Exclui um imóvel
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _imovelService.DeleteAsync(id, UsuarioId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Ativa um imóvel
        /// </summary>
        [HttpPut("{id}/ativar")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Ativar(Guid id)
        {
            try
            {
                await _imovelService.AtivarAsync(id, UsuarioId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Desativa um imóvel
        /// </summary>
        [HttpPut("{id}/desativar")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Desativar(Guid id)
        {
            try
            {
                await _imovelService.DesativarAsync(id, UsuarioId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Busca imóveis por termo
        /// </summary>
        [HttpGet("buscar")]
        [ProducesResponseType(typeof(IEnumerable<ImovelDto>), 200)]
        public async Task<IActionResult> Search([FromQuery] string termo)
        {
            try
            {
                var imoveis = await _imovelService.SearchAsync(termo, UsuarioId);
                return Ok(imoveis);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}