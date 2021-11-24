using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace JobReport
{
    internal class Method_Query
    {

        OracleConnection1 Or = new OracleConnection1();




        // GetApplicationTimeLog

        public List<log_API_for_Time> GetApplicationTimeLog(double unix_start, double unix_stop)
        {
            string query = string.Format("SELECT ApplicationDisplayName as ApplicationName, COUNT(ApplicationDisplayName) as HIT FROM APIRequestLog INNER JOIN cfg_applicationdisplayname on cfg_applicationdisplayname.applicationname=apirequestlog.applicationname and apirequestlog.applicationowner = cfg_applicationdisplayname.applicationowner WHERE requesttimestamp between {0:00} and {1:00}  GROUP bY ApplicationDisplayName", unix_start, unix_stop);
            DataTable dt = Or.ExecuteQuery(query);
            List<log_API_for_Time> appUsage = new List<log_API_for_Time>();
            appUsage = (from DataRow dr in dt.Rows
                        select new log_API_for_Time()
                        {
                            ApplicationName = dr["ApplicationName"].ToString(),
                            HIT = int.Parse(dr["HIT"].ToString())
                        }).ToList();
            return appUsage;
        }

        // GetAverageTime

        public List<AverageTime> GetAverageTime(double unix_start, double unix_stop)
        {
            string query = string.Format("SELECT ROUND(AVG(SERVICETIME),2) as avgservice,ROUND(AVG(Backendtime),2) as avgbackend,ROUND(AVG(Responsetime),2)as avgresponse FROM APIRequestLog WHERE requesttimestamp between {0} and {1}", unix_start, unix_stop);
            DataTable dt = Or.ExecuteQuery(query);
            List<AverageTime> appUsage = new List<AverageTime>();
            appUsage = (from DataRow dr in dt.Rows
                        select new AverageTime()
                        {
                            AGVservice = double.Parse(dr["avgservice"].ToString()),
                            AGVbackend = double.Parse(dr["avgbackend"].ToString()),
                            AGVresponse = double.Parse(dr["avgresponse"].ToString())

                        }).ToList();
            return appUsage;
        }

        // Timestamprequest

        public List<Timestamprequest> GetTimestamprequest(double unix_start, double unix_stop)
        {
            string query = string.Format("SELECT requesttimestamp,responsecode,servicetime FROM apirequestlog WHERE requesttimestamp between {0} and {1}", unix_start, unix_stop);
            DataTable dt = Or.ExecuteQuery(query);
            List<Timestamprequest> appUsage = new List<Timestamprequest>();
            appUsage = (from DataRow dr in dt.Rows
                        select new Timestamprequest()
                        {
                            Requesttimestamp = dr["Requesttimestamp"].ToString(),
                            Responsecode = int.Parse(dr["Responsecode"].ToString()),
                            Servicetime = int.Parse(dr["Servicetime"].ToString())

                        }).ToList();
            return appUsage;
        }
    }









    public class log_API_for_Time
    {
        public string ApplicationName { get; set; }
        public int HIT { get; set; }
    }

    public class AverageTime
    {
        public double AGVservice { get; set; }
        public double AGVbackend { get; set; }
        public double AGVresponse { get; set; }
    }

    public class Timestamprequest
    {
        public String Requesttimestamp { get; set; }
        public int Responsecode { get; set; }
        public int Servicetime { get; set; }
    }

}
