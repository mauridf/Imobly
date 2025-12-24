namespace Imobly.API.Middleware
{
    public class HttpCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HttpCacheMiddleware> _logger;

        public HttpCacheMiddleware(RequestDelegate next, ILogger<HttpCacheMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Apenas para métodos GET
            if (context.Request.Method != "GET")
            {
                await _next(context);
                return;
            }

            // Configurar headers de cache baseado no endpoint
            var cacheConfig = GetCacheConfiguration(context.Request.Path);

            if (cacheConfig.Duration > TimeSpan.Zero)
            {
                context.Response.Headers["Cache-Control"] =
                    $"public, max-age={(int)cacheConfig.Duration.TotalSeconds}";

                if (cacheConfig.IsImmutable)
                {
                    context.Response.Headers["Cache-Control"] += ", immutable";
                }

                context.Response.Headers["Vary"] = "Accept-Encoding";

                _logger.LogDebug("Cache configurado para {Path}: {Duration}s",
                    context.Request.Path, cacheConfig.Duration.TotalSeconds);
            }
            else
            {
                // Desabilitar cache para endpoints sensíveis
                context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
                context.Response.Headers["Pragma"] = "no-cache";
                context.Response.Headers["Expires"] = "0";
            }

            await _next(context);
        }

        private (TimeSpan Duration, bool IsImmutable) GetCacheConfiguration(PathString path)
        {
            return path.Value?.ToLower() switch
            {
                // Dados estáticos - cache longo
                var p when p.Contains("/images/") || p.Contains("/assets/") =>
                    (TimeSpan.FromDays(30), true),

                // Dados pouco alterados - cache médio
                var p when p.Contains("/api/imoveis") && !p.Contains("/buscar") =>
                    (TimeSpan.FromMinutes(5), false),

                // Dados dinâmicos - cache curto
                var p when p.Contains("/api/dashboard") =>
                    (TimeSpan.FromSeconds(30), false),

                // Dados sensíveis - sem cache
                var p when p.Contains("/api/usuarios/me") ||
                          p.Contains("/api/auth") ||
                          p.Contains("/api/contratos") =>
                    (TimeSpan.Zero, false),

                // Padrão
                _ => (TimeSpan.FromSeconds(60), false)
            };
        }
    }

    public static class HttpCacheMiddlewareExtensions
    {
        public static IApplicationBuilder UseHttpCache(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpCacheMiddleware>();
        }
    }
}