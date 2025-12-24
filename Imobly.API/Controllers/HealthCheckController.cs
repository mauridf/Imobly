using Imobly.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Imobly.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous] // Adicionar esta linha
    public class HealthCheckController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HealthCheckController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Testar conexão com o banco
                var canConnect = await _context.Database.CanConnectAsync();

                return Ok(new
                {
                    status = "Healthy",
                    timestamp = DateTime.UtcNow,
                    database = canConnect ? "Connected" : "Disconnected",
                    message = "API is running",
                    version = "1.0.0"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "Unhealthy",
                    timestamp = DateTime.UtcNow,
                    error = ex.Message,
                    message = "API is running with errors"
                });
            }
        }
    }
}