using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace SISLogger.Core
{
    public static class WebHelper
    {
        public static void LogWebUsage(string product, string layer, string activityName, Microsoft.AspNetCore.Http.HttpContext context, Dictionary<string, object> additionalInfo = null)
        {
            LogDetail details = GetWebDetails(product, layer, activityName, context, additionalInfo);
            Logger.WriteUsage(details);
        }

        public static void LogWebDiagnostic(string product, string layer, string message, Microsoft.AspNetCore.Http.HttpContext context, Dictionary<string, object> diagnosticInfo = null)
        {
            LogDetail details = GetWebDetails(product, layer, message, context, diagnosticInfo);
            Logger.WriteDiagnostic(details);
        }

        public static void LogWebError(string product, string layer, Exception exception, Microsoft.AspNetCore.Http.HttpContext context)
        {
            LogDetail details = GetWebDetails(product, layer, null, context, null);
            details.Exception = exception;

            Logger.WriteError(details);
        }

        internal static LogDetail GetWebDetails(string product, string layer, string activityName, HttpContext context, Dictionary<string, object> additionalInfo)
        {
            var detail = new LogDetail
            {
                Product = product,
                Layer = layer,
                Messages = activityName,
                Hostname = Environment.MachineName,
                CorrelationId = context.TraceIdentifier,
                AdditionalInfo = additionalInfo ?? new Dictionary<string, object>()
            };

            GetUserData(detail, context);
            GetRequestData(detail, context);

            return detail;
        }

        private static void GetRequestData(LogDetail detail, HttpContext context)
        {
            var request = context.Request;
            if (request != null)
            {
                detail.Location = request.Path;
                detail.AdditionalInfo.Add("UserAgent", request.Headers["User-Agent"]);
                detail.AdditionalInfo.Add("Languages", request.Headers["Accept-Language"]);

                var qdict = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(request.QueryString.ToString());
                foreach (var key in qdict.Keys)
                {
                    detail.AdditionalInfo.Add($"QueryString-{key}", qdict[key]);
                }
            }
        }

        private static void GetUserData(LogDetail detail, HttpContext context)
        {
            var userId = "";
            var userName = "";
            var user = context.User;
            if (user != null)
            {
                var i = 1;
                foreach (var claim in user.Claims)
                {
                    if (claim.Type == System.Security.Claims.ClaimTypes.NameIdentifier)
                    {
                        userId = claim.Value;
                    }
                    else if (claim.Type == "name")
                    {
                        userName = claim.Value;
                    }
                    else
                    {
                        detail.AdditionalInfo.Add(string.Format("UserClaim-{0}-{1}", i++, claim.Type), claim.Value);
                    }
                }
            }

            detail.UserId = userId;
            detail.UserName = userName;
        }
    }
}
