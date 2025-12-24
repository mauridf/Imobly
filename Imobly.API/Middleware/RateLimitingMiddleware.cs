using System.Threading.RateLimiting;

namespace Imobly.API.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private static readonly Dictionary<string, RateLimitInfo> _rateLimits = new();
        private static readonly SemaphoreSlim _semaphore = new(1, 1);

        public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientId = GetClientIdentifier(context);
            var limitConfig = GetLimitConfiguration(context.Request.Path);

            // Usar semaphore ao invés de lock para async
            await _semaphore.WaitAsync();
            try
            {
                if (!_rateLimits.ContainsKey(clientId))
                {
                    _rateLimits[clientId] = new RateLimitInfo
                    {
                        LastRequest = DateTime.UtcNow,
                        RequestCount = 0
                    };
                }

                var rateLimit = _rateLimits[clientId];
                var timeSinceLastRequest = DateTime.UtcNow - rateLimit.LastRequest;

                // Reset contador se passou o período
                if (timeSinceLastRequest > limitConfig.Window)
                {
                    rateLimit.RequestCount = 0;
                    rateLimit.LastRequest = DateTime.UtcNow;
                }

                // Verificar se excedeu o limite
                if (rateLimit.RequestCount >= limitConfig.MaxRequests)
                {
                    await HandleRateLimitExceeded(context, rateLimit, limitConfig, clientId);
                    return;
                }

                rateLimit.RequestCount++;
                rateLimit.LastRequest = DateTime.UtcNow;
            }
            finally
            {
                _semaphore.Release();
            }

            await _next(context);
        }

        private async Task HandleRateLimitExceeded(HttpContext context, RateLimitInfo rateLimit,
            (int MaxRequests, TimeSpan Window) limitConfig, string clientId)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = "application/json";

            var retryAfter = (rateLimit.LastRequest.Add(limitConfig.Window) - DateTime.UtcNow).TotalSeconds;
            context.Response.Headers["Retry-After"] = retryAfter.ToString("0");

            var response = new
            {
                type = "https://tools.ietf.org/html/rfc6585#section-4",
                title = "Muitas requisições.",
                status = StatusCodes.Status429TooManyRequests,
                detail = $"Limite de {limitConfig.MaxRequests} requisições por {limitConfig.Window.TotalSeconds} segundos excedido.",
                instance = context.Request.Path
            };

            await context.Response.WriteAsJsonAsync(response);

            _logger.LogWarning("Rate limit excedido para cliente {ClientId} no endpoint {Endpoint}",
                clientId, context.Request.Path);
        }

        private string GetClientIdentifier(HttpContext context)
        {
            // Prioridade: API Key > User ID > IP Address
            var apiKey = context.Request.Headers["X-API-Key"].FirstOrDefault();
            if (!string.IsNullOrEmpty(apiKey))
                return $"APIKEY_{apiKey}";

            var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
                return $"USER_{userId}";

            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            return $"IP_{ipAddress}";
        }

        private (int MaxRequests, TimeSpan Window) GetLimitConfiguration(PathString path)
        {
            // Configurações diferentes por tipo de endpoint
            return path.Value switch
            {
                var p when p.Contains("/auth/login", StringComparison.OrdinalIgnoreCase) =>
                    (5, TimeSpan.FromMinutes(1)),    // 5 tentativas de login por minuto
                var p when p.Contains("/auth/", StringComparison.OrdinalIgnoreCase) =>
                    (10, TimeSpan.FromMinutes(5)),   // 10 registros a cada 5 minutos
                var p when p.Contains("/api/", StringComparison.OrdinalIgnoreCase) =>
                    (100, TimeSpan.FromMinutes(1)),  // 100 requisições API por minuto
                _ => (60, TimeSpan.FromMinutes(1))   // Padrão: 60 requisições por minuto
            };
        }

        private class RateLimitInfo
        {
            public DateTime LastRequest { get; set; }
            public int RequestCount { get; set; }
        }
    }

    public static class RateLimitingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitingMiddleware>();
        }
    }
}