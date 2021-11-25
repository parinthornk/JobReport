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
        

        /*
        public void Connection()
        {

            Oracle.ManagedDataAccess.Client.OracleConnection con = new Oracle.ManagedDataAccess.Client.OracleConnection();
            //using connection string attributes to connect to Oracle Database
            con.ConnectionString = ConnString;
            con.Open();
            Console.WriteLine("Connected to Oracle" + con.ServerVersion);

        }
        */

        public DataTable ExecuteQuery(string query)
        {
            Oracle.ManagedDataAccess.Client.OracleConnection conn = new Oracle.ManagedDataAccess.Client.OracleConnection();
            //using connection string attributes to connect to Oracle Database
            conn.ConnectionString = Program.ConnectionString;

            {
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

}
