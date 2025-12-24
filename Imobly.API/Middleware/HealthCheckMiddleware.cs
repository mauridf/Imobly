using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Imobly.API.Middleware
{
    public class HealthCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HealthCheckService _healthCheckService;

        public HealthCheckMiddleware(RequestDelegate next, HealthCheckService healthCheckService)
        {
            _next = next;
            _healthCheckService = healthCheckService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.Equals("/health/detailed", StringComparison.OrdinalIgnoreCase))
            {
                var healthReport = await _healthCheckService.CheckHealthAsync();

                context.Response.ContentType = "application/json";

                var result = new
                {
                    status = healthReport.Status.ToString(),
                    totalDuration = healthReport.TotalDuration.TotalSeconds,
                    entries = healthReport.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        duration = e.Value.Duration.TotalSeconds,
                        description = e.Value.Description,
                        data = e.Value.Data,
                        exception = e.Value.Exception?.Message
                    })
                };

                await context.Response.WriteAsJsonAsync(result);
                return;
            }

            await _next(context);
        }
    }

    public static class HealthCheckMiddlewareExtensions
    {
        public static IApplicationBuilder UseDetailedHealthChecks(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HealthCheckMiddleware>();
        }
    }
}