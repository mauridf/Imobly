using System.Net;
using System.Text.Json;

namespace Imobly.API.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                title = "Ocorreu um erro interno no servidor.",
                status = (int)HttpStatusCode.InternalServerError,
                detail = _env.IsDevelopment() ? exception.Message : "Entre em contato com o suporte.",
                instance = context.Request.Path,
                traceId = context.TraceIdentifier
            };

            // Mapear exceções específicas para códigos HTTP apropriados
            switch (exception)
            {
                case KeyNotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response = new
                    {
                        type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                        title = "Recurso não encontrado.",
                        status = (int)HttpStatusCode.NotFound,
                        detail = exception.Message,
                        instance = context.Request.Path,
                        traceId = context.TraceIdentifier
                    };
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response = new
                    {
                        type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                        title = "Acesso não autorizado.",
                        status = (int)HttpStatusCode.Unauthorized,
                        detail = exception.Message,
                        instance = context.Request.Path,
                        traceId = context.TraceIdentifier
                    };
                    break;

                case ArgumentException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                        title = "Requisição inválida.",
                        status = (int)HttpStatusCode.BadRequest,
                        detail = exception.Message,
                        instance = context.Request.Path,
                        traceId = context.TraceIdentifier
                    };
                    break;

                case InvalidOperationException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                        title = "Operação inválida.",
                        status = (int)HttpStatusCode.BadRequest,
                        detail = exception.Message,
                        instance = context.Request.Path,
                        traceId = context.TraceIdentifier
                    };
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}