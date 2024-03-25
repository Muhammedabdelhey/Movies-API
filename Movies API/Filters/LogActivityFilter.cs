using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Movies_With_Reopsitory_Pattren.Filters
{
    // you should implement on of them only , if you implemented two interfaces IAsyncActionFilter will run
    // global action filter implement ActionFilter or IAsyncActionFilter
    public class LogActivityFilter : IActionFilter , IAsyncActionFilter
    {
        private readonly ILogger<LogActivityFilter> _logger;

        public LogActivityFilter(ILogger<LogActivityFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // short circuit, next request will not pass; program will terminate
            //context.Result = new NotFoundResult();
            _logger.LogInformation($"Executing action {context.ActionDescriptor.DisplayName} ");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation($"Action {context.ActionDescriptor.DisplayName} executed ");
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // this part equal to OnActionExecuting
            _logger.LogInformation($"Executing Async action {context.ActionDescriptor.DisplayName} ");
            await next();
            // this part equal to OnActionExecuted
            _logger.LogInformation($" Async Action {context.ActionDescriptor.DisplayName} executed ");
        }
    }
}
