using System.Text;

namespace VersionControlService.Extensions;

public static class ResponseLoggingExtensions
{
    public static IApplicationBuilder UseVersionControlResponseLogging(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            if (
                !context.Request.Path.StartsWithSegments("/update")
                && !context.Request.Path.StartsWithSegments("/download")
            )
            {
                await next();
                return;
            }

            var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("VersionControlService.Response");

            var originalBodyStream = context.Response.Body;
            await using var responseBuffer = new MemoryStream();
            context.Response.Body = responseBuffer;

            try
            {
                await next();

                responseBuffer.Position = 0;
                var responseBody = await new StreamReader(responseBuffer, Encoding.UTF8, leaveOpen: true)
                    .ReadToEndAsync();
                responseBuffer.Position = 0;
                await responseBuffer.CopyToAsync(originalBodyStream);

                var location = context.Response.Headers.Location.ToString();
                logger.LogInformation(
                    "HTTP {Method} {Path} => {StatusCode} ContentType={ContentType} Location={Location} Body={Body}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    context.Response.ContentType ?? string.Empty,
                    string.IsNullOrWhiteSpace(location) ? string.Empty : location,
                    string.IsNullOrWhiteSpace(responseBody) ? string.Empty : responseBody
                );
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        });
    }
}