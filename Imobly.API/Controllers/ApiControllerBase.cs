using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Imobly.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected Guid UsuarioId
        {
            get
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    throw new UnauthorizedAccessException("Usuário não autenticado");
                }
                return userId;
            }
        }

        protected string UsuarioEmail
        {
            get
            {
                return User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
            }
        }

        protected string UsuarioNome
        {
            get
            {
                return User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            }
        }

        protected IActionResult HandleException(Exception ex)
        {
            return ex switch
            {
                KeyNotFoundException => NotFound(new { error = ex.Message }),
                UnauthorizedAccessException => Unauthorized(new { error = ex.Message }),
                ArgumentException => BadRequest(new { error = ex.Message }),
                InvalidOperationException => BadRequest(new { error = ex.Message }),
                _ => StatusCode(500, new { error = "Ocorreu um erro interno no servidor" })
            };
        }

        protected IActionResult OkOrNotFound(object? result)
        {
            if (result == null)
                return NotFound(new { error = "Recurso não encontrado" });

            return Ok(result);
        }
    }
}