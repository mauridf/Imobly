using Imobly.Application.DTOs.Autenticacao;
using Imobly.Application.DTOs.Usuarios;
using Imobly.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imobly.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Realiza login no sistema
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 401)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Registra um novo usuário
        /// </summary>
        [HttpPost("registrar")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UsuarioDto), 201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public async Task<IActionResult> Registrar([FromBody] RegistrarRequest request)
        {
            try
            {
                var result = await _authService.RegistrarAsync(request);
                return CreatedAtAction(nameof(Registrar), result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Altera a senha do usuário logado
        /// </summary>
        [HttpPut("alterar-senha")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 401)]
        public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaDto request)
        {
            try
            {
                // Obter ID do usuário do token JWT
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var usuarioId))
                {
                    return Unauthorized(new { error = "Usuário não autenticado" });
                }

                await _authService.AlterarSenhaAsync(usuarioId, request);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Verifica se o token JWT é válido
        /// </summary>
        [HttpGet("validar")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult ValidarToken()
        {
            // Se chegou aqui, o token é válido (middleware JWT já validou)
            return Ok(new { message = "Token válido" });
        }
    }
}