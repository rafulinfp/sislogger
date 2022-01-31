using Serilog;
using Serilog.Events;
using System;
using System.Data.SqlClient;

namespace SISLogger.Core
{
    public static class Logger
    {
        private static readonly ILogger _perfLogger;
        private static readonly ILogger _usageLogger;
        private static readonly ILogger _errorLogger;
        private static readonly ILogger _diagnosticLogger;

        static Logger()
        {
            _perfLogger = new LoggerConfiguration()
                //.WriteTo.File(path: Environment.GetEnvironmentVariable("LOGFILE_PERF"))
                .WriteTo.Elasticsearch(Environment.GetEnvironmentVariable("ELASTICSEARCH_URL"),
                    indexFormat: "perf-{0:yyyy.MM.dd}",
                    inlineFields: true)
                .CreateLogger();

            _usageLogger = new LoggerConfiguration()
                 //.WriteTo.File(path: Environment.GetEnvironmentVariable("ELASTICSEARCH_URL"))
                 .WriteTo.Elasticsearch(Environment.GetEnvironmentVariable("ELASTICSEARCH_URL"),
                    indexFormat: "usage-{0:yyyy.MM.dd}",
                    inlineFields: true)
               .CreateLogger();

            _errorLogger = new LoggerConfiguration()
                 //.WriteTo.File(path: Environment.GetEnvironmentVariable("LOGFILE_ERROR"))
                 .WriteTo.Elasticsearch(Environment.GetEnvironmentVariable("ELASTICSEARCH_URL"),
                    indexFormat: "error-{0:yyyy.MM.dd}",
                    inlineFields: true)
               .CreateLogger();

            _diagnosticLogger = new LoggerConfiguration()
                 //.WriteTo.File(path: Environment.GetEnvironmentVariable("LOGFILE_DIAG"))
                 .WriteTo.Elasticsearch(Environment.GetEnvironmentVariable("ELASTICSEARCH_URL"),
                    indexFormat: "diagnostic-{0:yyyy.MM.dd}",
                    inlineFields: true)
               .CreateLogger();
        }

        public static void WritePerf(LogDetail infoToLog)
        {
            //_perfLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);
            _perfLogger.Write(LogEventLevel.Information,
                "{Timestamp}{Messages}{Product}{Layer}{Location}{Hostname}{UserId}{UserName}{CustomerId}{CustomerName}{ElapsedMillisenconds}{Exception}{CorrelationId}{AdditionalInfo}",
                infoToLog.Timestamp, 
                infoToLog.Messages,
                infoToLog.Product,
                infoToLog.Layer,
                infoToLog.Location,
                infoToLog.Hostname,
                infoToLog.UserId,
                infoToLog.UserName,
                infoToLog.CustomerId,
                infoToLog.CustomerName,
                infoToLog.ElapsedMillisenconds,
                infoToLog.Exception?.ToString(),
                infoToLog.CorrelationId,
                infoToLog.AdditionalInfo);
        }
        public static void WriteUsage(LogDetail infoToLog)
        {
            //_usageLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);
            _usageLogger.Write(LogEventLevel.Information,
                "{Timestamp}{Messages}{Product}{Layer}{Location}{Hostname}{UserId}{UserName}{CustomerId}{CustomerName}{ElapsedMillisenconds}{Exception}{CorrelationId}{AdditionalInfo}",
                infoToLog.Timestamp,
                infoToLog.Messages,
                infoToLog.Product,
                infoToLog.Layer,
                infoToLog.Location,
                infoToLog.Hostname,
                infoToLog.UserId,
                infoToLog.UserName,
                infoToLog.CustomerId,
                infoToLog.CustomerName,
                infoToLog.ElapsedMillisenconds,
                infoToLog.Exception?.ToString(),
                infoToLog.CorrelationId,
                infoToLog.AdditionalInfo);
        }
        public static void WriteError(LogDetail infoToLog)
        {
            if (infoToLog.Exception != null)
            {
                var procName = FindProcName(infoToLog.Exception);
                infoToLog.Location = string.IsNullOrEmpty(procName)
                    ? infoToLog.Location
                    : procName;
                infoToLog.Messages = GetMessageFromException(infoToLog.Exception);
            }
            //_errorLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);
            _errorLogger.Write(LogEventLevel.Information,
                "{Timestamp}{Messages}{Product}{Layer}{Location}{Hostname}{UserId}{UserName}{CustomerId}{CustomerName}{ElapsedMillisenconds}{Exception}{CorrelationId}{AdditionalInfo}",
                infoToLog.Timestamp,
                infoToLog.Messages,
                infoToLog.Product,
                infoToLog.Layer,
                infoToLog.Location,
                infoToLog.Hostname,
                infoToLog.UserId,
                infoToLog.UserName,
                infoToLog.CustomerId,
                infoToLog.CustomerName,
                infoToLog.ElapsedMillisenconds,
                infoToLog.Exception?.ToString(),
                infoToLog.CorrelationId,
                infoToLog.AdditionalInfo);
        }
        public static void WriteDiagnostic(LogDetail infoToLog)
        {
            var writeDiagnostics = Convert.ToBoolean(Environment.GetEnvironmentVariable("DIAGNOSTICS_ON"));
            if (!writeDiagnostics)
            {
                return;
            }
            // _diagnosticLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);
            _diagnosticLogger.Write(LogEventLevel.Information,
                "{Timestamp}{Messages}{Product}{Layer}{Location}{Hostname}{UserId}{UserName}{CustomerId}{CustomerName}{ElapsedMillisenconds}{Exception}{CorrelationId}{AdditionalInfo}",
                infoToLog.Timestamp,
                infoToLog.Messages,
                infoToLog.Product,
                infoToLog.Layer,
                infoToLog.Location,
                infoToLog.Hostname,
                infoToLog.UserId,
                infoToLog.UserName,
                infoToLog.CustomerId,
                infoToLog.CustomerName,
                infoToLog.ElapsedMillisenconds,
                infoToLog.Exception?.ToString(),
                infoToLog.CorrelationId,
                infoToLog.AdditionalInfo);
        }

        private static string GetMessageFromException(Exception ex)
        {
            if (ex.InnerException != null)
            {
                return GetMessageFromException(ex.InnerException);
            }
            return ex.Message;
        }
        private static string FindProcName(Exception ex)
        {
            var sqlEx = ex as SqlException;
            if (sqlEx != null)
            {
                var procName = sqlEx.Procedure;
                if (!string.IsNullOrEmpty(procName))
                {
                    return procName;
                }
            }
            if (!string.IsNullOrEmpty((string)ex.Data["Procedure"]))
            {
                return (string)ex.Data["Procedure"];
            }
            if (ex.InnerException != null)
            {
                return FindProcName(ex.InnerException);
            }
            return null;
        }
    }
}
