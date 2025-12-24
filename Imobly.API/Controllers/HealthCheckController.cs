using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Imobly.Infrastructure.Data;

namespace Imobly.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                    message = "API is running"
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