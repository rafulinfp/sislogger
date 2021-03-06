using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace SISLogger.Core.Attributes
{
    public class TrackerPerformanceFilter : IActionFilter
    {
        private PerfTracker _tracker;
        private string _product, _layer;

        public TrackerPerformanceFilter(string product, string layer)
        {
            _product = product;
            _layer = layer;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if(_tracker!= null)
            {
                _tracker.Stop();
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var activity = $"{request.Path} {request.Method}";

            var dict = new Dictionary<string, object>();
            foreach (var key in context.RouteData.Values?.Keys)
            {
                dict.Add($"RouteData-{key}", (string)context.RouteData.Values[key]);
            }

            var details = WebHelper.GetWebDetails(_product, _layer, activity, context.HttpContext, dict);

            _tracker = new PerfTracker(details);
        }
    }
}
