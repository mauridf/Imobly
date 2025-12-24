namespace Imobly.API.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiKeyMiddleware> _logger;

        public ApiKeyMiddleware(
            RequestDelegate next,
            IConfiguration configuration,
            ILogger<ApiKeyMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Endpoints que não requerem API Key
            if (ShouldSkipApiKeyValidation(context))
            {
                await _next(context);
                return;
            }

            // Verificar API Key no header
            if (!context.Request.Headers.TryGetValue("X-API-Key", out var extractedApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                    title = "API Key não fornecida.",
                    status = StatusCodes.Status401Unauthorized,
                    detail = "Por favor, forneça uma API Key válida no header 'X-API-Key'.",
                    instance = context.Request.Path
                });

                _logger.LogWarning("Tentativa de acesso sem API Key: {Path}", context.Request.Path);
                return;
            }

            // Validar API Key
            var validApiKeys = _configuration.GetSection("ApiKeys").Get<string[]>() ?? Array.Empty<string>();

            if (!validApiKeys.Contains(extractedApiKey.ToString()))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                    title = "API Key inválida.",
                    status = StatusCodes.Status401Unauthorized,
                    detail = "A API Key fornecida é inválida ou expirou.",
                    instance = context.Request.Path
                });

                _logger.LogWarning("Tentativa de acesso com API Key inválida: {Path} | Key: {ApiKey}",
                    context.Request.Path, extractedApiKey);
                return;
            }

            // API Key válida - registrar uso
            _logger.LogInformation("Acesso autorizado com API Key para: {Path}", context.Request.Path);

            await _next(context);
        }

        private bool ShouldSkipApiKeyValidation(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "";

            // Endpoints públicos
            var publicEndpoints = new[]
            {
                "/api/healthcheck",
                "/api/auth/login",
                "/api/auth/registrar",
                "/swagger",
                "/favicon.ico"
            };

            return publicEndpoints.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
        }
    }

    public static class ApiKeyMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyMiddleware>();
        }
    }
}