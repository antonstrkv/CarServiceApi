using System.Diagnostics;

namespace WebApi.Middlewares
{
    public class CarRequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CarRequestLoggingMiddleware> _logger;

        public CarRequestLoggingMiddleware(RequestDelegate next, ILogger<CarRequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
            {
                var stopwatch = Stopwatch.StartNew();

                _logger.LogInformation("Incoming request: {Method} {Path}", context.Request.Method, context.Request.Path);

                await _next(context);

                stopwatch.Stop();

                _logger.LogInformation(
                    "Completed request: {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds
                );
            }
            else
            {
                await _next(context);
            }
        }
    }
}
