using Imobly.Application.DTOs.Usuarios;
using Imobly.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imobly.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ApiControllerBase
    {
        private readonly IAuthService _authService;

        public UsuariosController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Obtém os dados do usuário atual
        /// </summary>
        [HttpGet("me")]
        [ProducesResponseType(typeof(UsuarioDto), 200)]
        [ProducesResponseType(401)]
        public IActionResult GetMe()
        {
            var usuario = new UsuarioDto
            {
                Id = UsuarioId,
                Nome = UsuarioNome,
                Email = UsuarioEmail,
                // Telefone seria obtido do banco de dados em um cenário real
            };

            return Ok(usuario);
        }

        /// <summary>
        /// Atualiza os dados do usuário
        /// </summary>
        [HttpPut("me")]
        [ProducesResponseType(typeof(UsuarioDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> UpdateMe([FromBody] AtualizarUsuarioDto dto)
        {
            try
            {
                // Em um cenário real, você teria um serviço para atualizar usuário
                // Por enquanto, retornamos os dados atualizados
                var usuarioAtualizado = new UsuarioDto
                {
                    Id = UsuarioId,
                    Nome = dto.Nome ?? UsuarioNome,
                    Email = UsuarioEmail, // Email não pode ser alterado
                    Telefone = dto.Telefone,
                    CriadoEm = DateTime.UtcNow // Em um cenário real, viria do banco
                };

                return Ok(usuarioAtualizado);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Altera a senha do usuário
        /// </summary>
        [HttpPut("alterar-senha")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaDto dto)
        {
            try
            {
                await _authService.AlterarSenhaAsync(UsuarioId, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}