using SISLogger.Core;
using SISLogger.Data.CustomAdo;
using System;
using System.Data.SqlClient;

namespace SISLogger.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var fd = GetLogDetail("starting application", null);
            Logger.WriteDiagnostic(fd);

            var tracker = new PerfTracker("LoggerConsole_Main", "", fd.UserName, fd.Location, fd.Product, fd.Layer);

            try
            {
                var ex = new Exception("Something went wrong!");
                ex.Data.Add("input param", "nothing to see here");
                throw ex;
            }
            catch (Exception ex)
            {
                fd = GetLogDetail("", ex);
                Logger.WriteError(fd);
            }

            fd = GetLogDetail("used logging console", null);
            Logger.WriteUsage(fd);

            fd = GetLogDetail("stopping app", null);
            Logger.WriteDiagnostic(fd);

            tracker.Stop();

            // try exception handlers in SQL
            //RAW ADO.NET
            using (var db = new SqlConnection("Server=(localdb)\\mssqllocaldb;Database=aspnet-DJme.Web-53bc9b9d-9d6a-45d4-8429-2a2761773502;Trusted_Connection=True;MultipleActiveResultSets=true"))
            {
                try
                {
                    db.Open();
                    var sp = new Sproc(db, "CreateNewCustomer");
                    sp.ExecNonQuery();
                }
                catch (Exception ex)
                {
                    fd = GetLogDetail("", ex);
                    Logger.WriteError(fd);
                }
            }
        }

        private static LogDetail GetLogDetail(string message, Exception ex)
        {
            return new LogDetail
            {
                Product = "Logger",
                Location = "LoggerConsole",
                Layer = "Job",
                UserName = Environment.UserName,
                Hostname = Environment.MachineName,
                Messages = message,
                Exception = ex
            };
        }
    }

}
