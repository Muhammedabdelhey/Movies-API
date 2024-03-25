namespace Movies_With_Reopsitory_Pattren.Middlewares
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private DateTime _lastRequestDate = DateTime.Now;
        private int _counter = 0;
        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (DateTime.Now.Subtract(_lastRequestDate).Seconds > 10)
            {
                // rest counter to one 
                _counter = 1;
                _lastRequestDate = DateTime.Now;
                await _next(context);
            }
            else
            {
                if (_counter > 5)
                {
                    // short circuit, next request will not pass; program will terminate
                    _counter = 0;
                    _lastRequestDate = DateTime.Now;
                    await context.Response.WriteAsync("Rate limit exceeded");
                }
                else
                {
                    // update counter and time to ensure limit not reached 
                    _counter++;
                    _lastRequestDate = DateTime.Now;
                    await _next(context);
                }
            }
        }
    }
}
