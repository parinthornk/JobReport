using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobReport
{
    internal class OracleConnection1
    {
        public DataTable ExecuteQuery(string query)
        {
            var conn = new OracleConnection();
            conn.ConnectionString = Program.ConnectionString;
            using (OracleCommand command = new OracleCommand(query, conn))
            {
                try
                {
                    conn.Open();
                    OracleDataAdapter da = new OracleDataAdapter(query, conn);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "Table1");
                    return ds.Tables[0];
                }
                catch (OracleException ex)
                {
                    Program.ErrorCount++;
                    Console.WriteLine(ex.Message);
                    return null;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}