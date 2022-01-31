using System;
using System.Data.SqlClient;
using System.Text;

namespace SISLogger.Data.CustomAdo
{
    public class Sproc
    {
        private SqlCommand Command { get; set; }

        public Sproc(SqlConnection db, string procName, int timeoutSeconds = 30)
        {
            Command = new SqlCommand(procName, db) { CommandType = System.Data.CommandType.StoredProcedure };
            if (timeoutSeconds != 30)
            {
                Command.CommandTimeout = timeoutSeconds;
            }
        }

        public void SetParam(string paramName, object value)
        {
            Command.Parameters.Add(new SqlParameter(paramName, value ?? DBNull.Value));
        }

        public int ExecNonQuery()
        {
            try
            {
                return Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw CreateProcedureException(ex);
            }
        }

        private Exception CreateProcedureException(Exception ex)
        {
            var newEx = new Exception("Store Procedure call failed!", ex);
            newEx.Data.Add("Procedure", Command.CommandText);
            newEx.Data.Add("ProcInputs", GetInputString());
            return newEx;
        }

        private string GetInputString()
        {
            var inString = new StringBuilder();
            foreach (SqlParameter item in Command.Parameters)
            {
                inString.Append($"{item.ParameterName}={item.Value}|");
            }
            return inString.ToString();
        }
    }
}
