using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Imobly.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;

        public HealthController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var report = await _healthCheckService.CheckHealthAsync();

            var result = new
            {
                status = report.Status.ToString(),
                totalDuration = report.TotalDuration,
                entries = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    duration = e.Value.Duration,
                    description = e.Value.Description,
                    exception = e.Value.Exception?.Message,
                    data = e.Value.Data
                })
            };

            return report.Status == HealthStatus.Healthy
                ? Ok(result)
                : StatusCode(503, result);
        }
    }
}