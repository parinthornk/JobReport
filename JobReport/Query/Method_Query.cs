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

            Console.WriteLine(query);
            
            DataTable dt = Or.ExecuteQuery(query);
            List<log_API_for_Time> appUsage = new List<log_API_for_Time>();
            appUsage = (from DataRow dr in dt.Rows
                        select new log_API_for_Time()
                        {
                            ApplicationName = dr["ApplicationName"].ToString(),
                            HIT = int.Parse(dr["HIT"].ToString())
                        }).ToList();
            return new List<log_API_for_Time>(appUsage);
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
            return new List<AverageTime>(appUsage);
        }

        // GetTimestamprequest5minut

        public List<ResponseHit> GetTimestamprequest5minut(double unix_start, double unix_stop)
        {
            string query = string.Format("SELECT requesttimestamp,responsecode,servicetime FROM apirequestlog WHERE requesttimestamp between {0} and {1} ", unix_start, unix_stop);
            DataTable dt = Or.ExecuteQuery(query);
            List<Response> appUsage = new List<Response>();
            appUsage = (from DataRow dr in dt.Rows
                        select new Response()
                        {
                            responseCode = int.Parse(dr["responsecode"].ToString()),
                            requestTimestamp = double.Parse(dr["requesttimestamp"].ToString()),
                            AvgServiceTime = int.Parse(dr["servicetime"].ToString())
                        }).ToList();
            var groups = appUsage.GroupBy(x =>
            {
                var stamp = x.requestTimestamp;
                stamp = stamp - (stamp % (5000 * 60));
                return stamp;
            }).Select(g => new ResponseHit { requestTimestamp = g.Key, HIT = g.Count(s => s.responseCode == 200), AvgServiceTime = string.Format("{0:n}", g.Average(x => x.AvgServiceTime)) }).OrderBy(x => x.requestTimestamp).ToList();

            // save for test
            var listString = new List<string>();
            foreach (var x in groups)
            {
                var text = x.AvgServiceTime + "|" + x.HIT + "|" + x.requestTimestamp;
                listString.Add(text);
            }
            System.IO.File.WriteAllLines("GetTimestamprequest5minut.txt", listString);

            return new List<ResponseHit>(groups);
        }

        public double GetTotalCost(string MONTH)
        {
            Console.WriteLine("In method GetTotalCost");

            string query = string.Format("SELECT PRICE FROM CFG_UNILIMITEDCHARGE WHERE MONTH ='{0}' ", MONTH);

            DataTable dt = Or.ExecuteQuery(query);
            DataRow dr = dt.Rows[0];
            double TotalPrice = double.Parse(dr["PRICE"].ToString());

            return TotalPrice;
        }
    }









    public class log_API_for_Time
    {
        public string ApplicationName { get; set; }
        public int HIT { get; set; }

        private class Sortable : IComparer<log_API_for_Time>
        {
            public int Compare(log_API_for_Time x, log_API_for_Time y)
            {
                if (x.HIT > y.HIT)
                {
                    return -1;
                }
                if (x.HIT < y.HIT)
                {
                    return +1;
                }
                return 0;
            }
        }

        public static List<log_API_for_Time> Sort(List<log_API_for_Time> unsorted)
        {
            var tmp = unsorted.ToArray();
            Array.Sort(tmp, new Sortable());
            return tmp.ToList();
        }
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

    public class Response
    {
        public double requestTimestamp { get; set; }
        public int responseCode { get; set; }
        public int AvgServiceTime { get; set; }
    }

    public class ResponseHit
    {
        public double requestTimestamp { get; set; }
        public int HIT { get; set; }

        public string AvgServiceTime { get; set; }
    }
}
