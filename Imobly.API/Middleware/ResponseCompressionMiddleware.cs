using System.IO.Compression;

namespace Imobly.API.Middleware
{
    public class ResponseCompressionMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseCompressionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var acceptEncoding = context.Request.Headers["Accept-Encoding"].ToString();

            if (ShouldCompressResponse(context, acceptEncoding))
            {
                var originalBodyStream = context.Response.Body;

                using var memoryStream = new MemoryStream();
                context.Response.Body = memoryStream;

                await _next(context);

                memoryStream.Seek(0, SeekOrigin.Begin);

                // Aplicar compressão baseada no header Accept-Encoding
                if (acceptEncoding.Contains("br", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.Headers["Content-Encoding"] = "br";
                    await CompressWithBrotli(memoryStream, originalBodyStream);
                }
                else if (acceptEncoding.Contains("gzip", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.Headers["Content-Encoding"] = "gzip";
                    await CompressWithGzip(memoryStream, originalBodyStream);
                }
                else if (acceptEncoding.Contains("deflate", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.Headers["Content-Encoding"] = "deflate";
                    await CompressWithDeflate(memoryStream, originalBodyStream);
                }
                else
                {
                    await memoryStream.CopyToAsync(originalBodyStream);
                }

                context.Response.Body = originalBodyStream;
            }
            else
            {
                await _next(context);
            }
        }

        private bool ShouldCompressResponse(HttpContext context, string acceptEncoding)
        {
            if (string.IsNullOrEmpty(acceptEncoding))
                return false;

            // Não comprimir pequenas respostas
            if (context.Response.ContentLength.HasValue && context.Response.ContentLength < 1024)
                return false;

            // Não comprimir certos tipos de conteúdo
            var contentType = context.Response.ContentType?.ToLower() ?? "";
            var excludedTypes = new[]
            {
                "image/", "video/", "audio/", "application/octet-stream",
                "application/pdf", "application/zip"
            };

            if (excludedTypes.Any(t => contentType.Contains(t)))
                return false;

            return true;
        }

        private async Task CompressWithGzip(Stream input, Stream output)
        {
            using var gzipStream = new GZipStream(output, CompressionLevel.Optimal, leaveOpen: true);
            await input.CopyToAsync(gzipStream);
        }

        private async Task CompressWithDeflate(Stream input, Stream output)
        {
            using var deflateStream = new DeflateStream(output, CompressionLevel.Optimal, leaveOpen: true);
            await input.CopyToAsync(deflateStream);
        }

        private async Task CompressWithBrotli(Stream input, Stream output)
        {
            using var brotliStream = new BrotliStream(output, CompressionLevel.Optimal, leaveOpen: true);
            await input.CopyToAsync(brotliStream);
        }
    }

    public static class ResponseCompressionMiddlewareExtensions
    {
        public static IApplicationBuilder UseResponseCompressionCustom(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseCompressionMiddleware>();
        }
    }
}