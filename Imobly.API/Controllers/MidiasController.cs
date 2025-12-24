using Imobly.Application.Interfaces;
using Imobly.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Imobly.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MidiasController : ApiControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public MidiasController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Obtém mídias de um imóvel
        /// </summary>
        [HttpGet("imovel/{imovelId}")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByImovel(Guid imovelId)
        {
            try
            {
                var imovel = await _unitOfWork.Imoveis.GetByIdAsync(imovelId);
                if (imovel == null || imovel.UsuarioId != UsuarioId)
                {
                    return NotFound(new { error = "Imóvel não encontrado ou acesso negado" });
                }

                var midias = await _unitOfWork.MidiasImoveis.FindAsync(m => m.ImovelId == imovelId);
                return Ok(midias);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Exclui uma mídia
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var midia = await _unitOfWork.MidiasImoveis.GetByIdAsync(id);
                if (midia == null)
                {
                    return NotFound(new { error = "Mídia não encontrada" });
                }

                var imovel = await _unitOfWork.Imoveis.GetByIdAsync(midia.ImovelId);
                if (imovel == null || imovel.UsuarioId != UsuarioId)
                {
                    return NotFound(new { error = "Acesso negado" });
                }

                _unitOfWork.MidiasImoveis.Remove(midia);
                await _unitOfWork.CompleteAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}