using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace WebApi.Filters
{
    public class ExecutionTimingFilter : IActionFilter
    {
        private readonly ILogger<ExecutionTimingFilter> _logger;
        private Stopwatch? _stopwatch;

        public ExecutionTimingFilter(ILogger<ExecutionTimingFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch?.Stop();
            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;

            _logger.LogInformation(
                "[INFO] {Method} {Path} — {StatusCode} — {Elapsed}ms",
                request.Method,
                request.Path,
                response.StatusCode,
                _stopwatch?.ElapsedMilliseconds ?? 0
            );
        }
    }
}
