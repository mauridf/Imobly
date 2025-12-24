using Imobly.Application.DTOs.Manutencoes;
using Imobly.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Imobly.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManutencoesController : ApiControllerBase
    {
        private readonly IManutencaoService _manutencaoService;

        public ManutencoesController(IManutencaoService manutencaoService)
        {
            _manutencaoService = manutencaoService;
        }

        /// <summary>
        /// Obtém manutenções por imóvel
        /// </summary>
        [HttpGet("imovel/{imovelId}")]
        [ProducesResponseType(typeof(IEnumerable<ManutencaoDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByImovel(Guid imovelId)
        {
            try
            {
                var manutencoes = await _manutencaoService.GetByImovelAsync(imovelId, UsuarioId);
                return Ok(manutencoes);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém uma manutenção pelo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ManutencaoDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var manutencao = await _manutencaoService.GetByIdAsync(id, UsuarioId);
                return OkOrNotFound(manutencao);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Cria uma nova manutenção
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ManutencaoDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CriarManutencaoDto dto)
        {
            try
            {
                var manutencao = await _manutencaoService.CreateAsync(dto, UsuarioId);
                return CreatedAtAction(nameof(GetById), new { id = manutencao.Id }, manutencao);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Atualiza uma manutenção existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ManutencaoDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] AtualizarManutencaoDto dto)
        {
            try
            {
                var manutencao = await _manutencaoService.UpdateAsync(id, dto, UsuarioId);
                return Ok(manutencao);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Exclui uma manutenção
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _manutencaoService.DeleteAsync(id, UsuarioId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Marca manutenção como feita
        /// </summary>
        [HttpPut("{id}/concluir")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Concluir(Guid id)
        {
            try
            {
                await _manutencaoService.MarcarComoFeitaAsync(id, UsuarioId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}