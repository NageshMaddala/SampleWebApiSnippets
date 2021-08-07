using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SampleSnippets.ActionFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RouterTimerFilterAttribute : ActionFilterAttribute
    {
        public const string _Header = "X-API-Timer";
        public const string _TimerPropertyName = "RouteTimerFilter_";
        public string TimerName = "";

        public RouterTimerFilterAttribute()
        {
        }

        /// <summary>
        /// Executed BEFORE the controller action method is called.
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var name = actionContext.ActionDescriptor.ActionName;
            actionContext.Request.Properties[_TimerPropertyName + name] = Stopwatch.StartNew();
            await base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext,
            CancellationToken cancellationToken)
        {
            string name = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;

            var timer = (Stopwatch)actionExecutedContext.Request.Properties[_TimerPropertyName + name];
            var time = timer.ElapsedMilliseconds;

            Trace.Write(actionExecutedContext.Request.Method + " "
                + actionExecutedContext.Request.RequestUri + " " +
                actionExecutedContext.ActionContext.ActionDescriptor.ActionName + " - Elapsed time for " + name + " was " + time + "\n");

            await base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);

            actionExecutedContext.Response.Headers.Add(_Header, time + " msec");
        }
    }
}