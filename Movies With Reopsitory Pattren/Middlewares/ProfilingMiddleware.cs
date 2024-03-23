using System.Diagnostics;

namespace Movies_With_Reopsitory_Pattren.Middlewares
{
    public class ProfilingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ProfilingMiddleware> _logger;

        //first  create constrctor have instance of RequestDelegate
        public ProfilingMiddleware(RequestDelegate next, ILogger<ProfilingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        // sec create InvokeAsync method that wall have logic 
        public async Task InvokeAsync(HttpContext context)
        {
            // handel request
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            await _next(context);
            // handle response
            stopWatch.Stop();
            _logger.LogInformation($"Request `{context.Request.Path}`, took {stopWatch.ElapsedMilliseconds}ms to finish");
        }
    }
}
