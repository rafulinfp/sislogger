using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace SISLogger.Core.Attributes
{
    public class TrackUsageAttribute : ActionFilterAttribute
    {
        private string _productName;
        private string _layerName;
        private string _name;

        public TrackUsageAttribute(string productName, string layerName, string name)
        {
            _productName = productName;
            _layerName = layerName;
            _name = name;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var dict = new Dictionary<string, object>();
            foreach (var key in context.RouteData.Values?.Keys)
            {
                dict.Add($"RouteData-{key}", (string)context.RouteData.Values[key]);
            }

            WebHelper.LogWebUsage(_productName, _layerName, _name, context.HttpContext, dict);
        }
    }
}
