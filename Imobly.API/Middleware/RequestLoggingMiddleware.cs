using System.Diagnostics;
using System.Text;

namespace Imobly.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var originalBodyStream = context.Response.Body;

            // Capturar request
            var request = await FormatRequest(context.Request);

            // Capturar response
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
                stopwatch.Stop();

                // Formatar response
                var response = await FormatResponse(context.Response);

                // Log detalhado
                LogRequestResponse(context, request, response, stopwatch.ElapsedMilliseconds);

                await responseBody.CopyToAsync(originalBodyStream);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Position = 0;

            // Não logar senhas ou tokens
            if (request.Path.Value.Contains("/auth/login") || request.Path.Value.Contains("/auth/registrar"))
            {
                bodyAsText = "[CONTEÚDO SENSÍVEL REMOVIDO DO LOG]";
            }

            return $"{request.Method} {request.Path}{request.QueryString} {bodyAsText}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return $"{response.StatusCode}: {text}";
        }

        private void LogRequestResponse(HttpContext context, string request, string response, long durationMs)
        {
            var logLevel = context.Response.StatusCode >= 500 ? LogLevel.Error :
                          context.Response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;

            var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
            var userEmail = context.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "Anonymous";

            _logger.Log(logLevel,
                "Request: {Method} {Path} by User:{UserId} ({UserEmail}) | " +
                "Status: {StatusCode} | Duration: {Duration}ms | " +
                "Request: {Request} | Response: {Response}",
                context.Request.Method,
                context.Request.Path,
                userId,
                userEmail,
                context.Response.StatusCode,
                durationMs,
                request,
                response.Length > 500 ? response.Substring(0, 500) + "..." : response);
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}