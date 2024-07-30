using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace Movies_With_Reopsitory_Pattren.Filters
{
    // scoped action filiter (for controller or action"method") implement ActionFilterAttribute
    public class LogSensitiveActionAttriburte:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Debug.WriteLine("Executing Sensitive Data !!!!!!!!!!!!!!!!!!!!!");
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Debug.WriteLine("Executing Sensitive Data !!!!!!!!!!!!!!!!!!!!!");
            await next();
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Debug.WriteLine("Sensitive Data Executed !!!!!!!!!!!!!!!!!!!!!!");
        }
    }
}
