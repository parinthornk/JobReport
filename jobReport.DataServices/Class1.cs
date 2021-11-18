using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;

namespace jobReport.DataServices
{
    public class AnalyticsData
    {
        public List<Package> GetPackage(string package)
        {
            string query = string.Format("SELECT * FROM cfg_ConsumerPackage where packagename='{0}' ", package);
            DataTable dt = ExecuteQuery(query);
            List<Package> appUsage = new List<Package>();
            appUsage = (from DataRow dr in dt.Rows
                        select new Package()
                        {
                            PackageName = dr["PackageName"].ToString(),
                            APILIMITSIZE = int.Parse(dr["APIESBLIMIT"].ToString()),
                            APIPRICE = int.Parse(dr["APIESBPRICE"].ToString()),
                            OVEREXCEEDPRICE = double.Parse(dr["OVEREXCEEDPRICE"].ToString())
                        }).ToList();
            return appUsage;
        }
        public List<PriceStruct> GetPriceStructer(string package, string from, string to)
        {
            List<PriceStruct> appUsage = new List<PriceStruct>();
            List<ApplicationUsage> getTotal = GetApplicationUsage(from, to);
            double x = getTotal.Select(y => y.HIT).Sum();
            if (!package.Equals("Unlimited"))
            {
                string query = string.Format("SELECT SUM(AGG_SUM_SUCCESSCOUNT) as Total,packagename,APIESBLimit,APIESBPRICE,OVEREXCEEDPRICE,CASE WHEN(SUM(AGG_SUM_SUCCESSCOUNT) > APIESBLimit) THEN(SUM(AGG_SUM_SUCCESSCOUNT) - APIESBLimit) ELSE 0 END as OVERLIMIT,CASE WHEN(SUM(AGG_SUM_SUCCESSCOUNT) > APIESBLimit) THEN(SUM(AGG_SUM_SUCCESSCOUNT) - APIESBLimit) * OVEREXCEEDPRICE ELSE 0 END as OVERLIMITPRICE,CASE WHEN(SUM(AGG_SUM_SUCCESSCOUNT) > APIESBLimit) THEN((SUM(AGG_SUM_SUCCESSCOUNT) - APIESBLimit) * OVEREXCEEDPRICE) + APIESBPRICE ELSE APIESBPRICE END as TotalPrice FROM APIM_REQCOUNTAGG_Days INNER JOIN cfg_applicationdisplayname on cfg_applicationdisplayname.applicationname = APIM_REQCOUNTAGG_Days.applicationname  LEFT JOIN cfg_consumerpackage ON cfg_consumerpackage.packagename = '{0}' WHERE AGG_TIMESTAMP between {1} and {2} GROUP BY cfg_consumerpackage.packagename, APIESBLimit, OVEREXCEEDPRICE, APIESBPRICE", package, getTimestamp(from), getEndTimestamp(to));
                DataTable dt = ExecuteQuery(query);
                if (dt.Rows.Count > 0)
                    appUsage = (from DataRow dr in dt.Rows
                                select new PriceStruct()
                                {
                                    PackageName = dr["PackageName"].ToString(),
                                    APIESBLimit = int.Parse(dr["APIESBLIMIT"].ToString()).ToString(),
                                    OVERLIMIT = int.Parse(dr["OVERLIMIT"].ToString()),
                                    OVERLIMITPRICE = double.Parse(dr["OVERLIMITPRICE"].ToString()),
                                    TotalPrice = double.Parse(dr["TotalPrice"].ToString()),
                                    Total = (int)x,
                                    PackagePrice = int.Parse(dr["APIESBPRICE"].ToString()),
                                    OVEREXCEEDPRICE = double.Parse(dr["OVEREXCEEDPRICE"].ToString())
                                }).ToList();
                else
                {
                    query = string.Format("SELECT 0 as Total,packagename,APIESBLimit,APIESBPRICE,OVEREXCEEDPRICE,0 as OVERLIMIT,0 as OVERLIMITPRICE, APIESBPRICE as TotalPrice FROM  cfg_consumerpackage WHERE packagename = '{0}'", package);
                    dt = ExecuteQuery(query);
                    appUsage = (from DataRow dr in dt.Rows
                                select new PriceStruct()
                                {
                                    PackageName = dr["PackageName"].ToString(),
                                    APIESBLimit = int.Parse(dr["APIESBLIMIT"].ToString()).ToString(),
                                    OVERLIMIT = int.Parse(dr["OVERLIMIT"].ToString()),
                                    OVERLIMITPRICE = double.Parse(dr["OVERLIMITPRICE"].ToString()),
                                    TotalPrice = double.Parse(dr["TotalPrice"].ToString()),
                                    Total = (int)x,
                                    PackagePrice = int.Parse(dr["APIESBPRICE"].ToString()),
                                    OVEREXCEEDPRICE = double.Parse(dr["OVEREXCEEDPRICE"].ToString())
                                }).ToList();
                }

            }
            else
            {
                int Month = int.Parse(from.Split('/')[0]);
                string query = string.Format("SELECT PRICE FROM CFG_UNILIMITEDCHARGE WHERE MONTH = " + Month.ToString());
                DataTable dt = ExecuteQuery(query);
                DataRow dr = dt.Rows[0];
                double TotalPrice = double.Parse(dr["PRICE"].ToString());
                query = string.Format("SELECT SUM(AGG_SUM_SUCCESSCOUNT) as Total,packagename  FROM APIM_REQCOUNTAGG_Days  LEFT JOIN cfg_consumerpackage ON cfg_consumerpackage.packagename = '{0}' WHERE AGG_TIMESTAMP between {1} and {2} GROUP BY cfg_consumerpackage.packagename, APIESBLimit, OVEREXCEEDPRICE, APIESBPRICE", package, getTimestamp(from), getEndTimestamp(to));
                dt = ExecuteQuery(query);
                appUsage = (from DataRow dx in dt.Rows
                            select new PriceStruct()
                            {
                                PackageName = "Unlimited",
                                Total = (int)x,
                                APIESBLimit = "Unlimited ",
                                TotalPrice = TotalPrice,
                                PackagePrice = TotalPrice
                            }).ToList();
            }
            return appUsage;
        }
        public List<ApplicationUsage> GetApplicationUsage(string from, string to)
        {
            string query = string.Format("SELECT ApplicationDisplayName as ApplicationName, COUNT(ApplicationDisplayName) as HIT FROM APIRequestLog INNER JOIN cfg_applicationdisplayname on cfg_applicationdisplayname.applicationname=apirequestlog.applicationname and apirequestlog.applicationowner = cfg_applicationdisplayname.applicationowner WHERE requesttimestamp between {0} and {1}  GROUP bY ApplicationDisplayName", getTimestamp(from), getEndTimestamp(to));
            DataTable dt = ExecuteQuery(query);
            List<ApplicationUsage> appUsage = new List<ApplicationUsage>();
            appUsage = (from DataRow dr in dt.Rows
                        select new ApplicationUsage()
                        {
                            ApplicationName = dr["ApplicationName"].ToString(),
                            HIT = int.Parse(dr["HIT"].ToString())
                        }).ToList();
            return appUsage;
        }
        public List<Error> GetErrorList(string from, string to, int pageSize, int PageNo)
        {
            string query = string.Format("SELECT TO_CHAR( FROM_TZ( CAST(DATE '1970-01-01' + (1/24/60/60/1000) *(requesttimestamp) AS TIMESTAMP), SESSIONTIMEZONE), 'MM/DD/YYYY HH24:MI') as requesttime,applicationname,apiname,apiresourcepath,responsecode from apirequestlog where responsecode <>200 and requesttimestamp between {0} and {1} order by requesttimestamp desc", getTimestamp(from), getEndTimestamp(to));
            DataTable dt = ExecuteQuery(query);
            List<Error> appUsage = new List<Error>();
            if (dt.Rows.Count != 0)
            {
                appUsage = (from DataRow dr in dt.Rows
                            select new Error()
                            {
                                applicationname = dr["applicationname"].ToString(),
                                requesttime = dr["requesttime"].ToString(),
                                responsecode = int.Parse(dr["responsecode"].ToString()),
                                apiname = dr["apiname"].ToString(),
                                apiresourcepath = dr["apiresourcepath"].ToString()
                            }).Skip(pageSize * (PageNo - 1))
      .Take(pageSize).ToList();
            }
            return appUsage;
        }
        public List<avgStatistic> GetAverageStatic(string from, string to)
        {
            string query = string.Format("SELECT ROUND(AVG(SERVICETIME),2)as avgservice,ROUND(AVG(Backendtime),2) as avgbackend,ROUND(AVG(Responsetime),2)as avgresponse FROM APIRequestLog WHERE requesttimestamp between {0} and {1}", getTimestamp(from), getEndTimestamp(to));
            DataTable dt = ExecuteQuery(query);

            List<avgStatistic> appUsage = new List<avgStatistic>();
            if (dt.Rows.Count > 0)
                appUsage = (from DataRow dr in dt.Rows
                            select new avgStatistic()
                            {
                                avgbackend = (dr["avgbackend"].ToString() == "") ? 0 : double.Parse(dr["avgbackend"].ToString()),
                                avgresponse = (dr["avgresponse"].ToString() == "") ? 0 : double.Parse(dr["avgresponse"].ToString()),
                                avgservice = (dr["avgservice"].ToString() == "") ? 0 : double.Parse(dr["avgservice"].ToString())
                            }).ToList();
            return appUsage;
        }
        public List<avgStatistic> getCPU(string from, string to)
        {
            string query = string.Format("SELECT ROUND(AVG(SERVICETIME),2)as avgservice,ROUND(AVG(Backendtime),2) as avgbackend,ROUND(AVG(Responsetime),2)as avgresponse FROM APIRequestLog WHERE requesttimestamp between {0} and {1}", getTimestamp(from), getEndTimestamp(to));
            DataTable dt = ExecuteQuery(query);

            List<avgStatistic> appUsage = new List<avgStatistic>();
            if (dt.Rows.Count > 0)
                appUsage = (from DataRow dr in dt.Rows
                            select new avgStatistic()
                            {
                                avgbackend = double.Parse(dr["avgbackend"].ToString()),
                                avgresponse = double.Parse(dr["avgresponse"].ToString()),
                                avgservice = double.Parse(dr["avgservice"].ToString())
                            }).ToList();
            return appUsage;
        }
        public double getTimestamp(string dateTime)
        {
            DateTime oDate = Convert.ToDateTime(dateTime);
            return (oDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds * 1000;
        }
        public double getEndTimestamp(string dateTime)
        {
            DateTime oDate = Convert.ToDateTime(dateTime);
            oDate = oDate.AddDays(1);
            return ((oDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds * 1000) - 1;
        }

        public List<ResponseHit> getResponseCode(string from, string to)
        {
            string query = string.Format("SELECT requesttimestamp,responsecode,servicetime FROM apirequestlog WHERE requesttimestamp between {0} and {1} ", getTimestamp(from), getEndTimestamp(to));
            DataTable dt = ExecuteQuery(query);
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
            })
.Select(g => new ResponseHit { requestTimestamp = g.Key, HIT = g.Count(s => s.responseCode == 200), AvgServiceTime = string.Format("{0:n}", g.Average(x => x.AvgServiceTime)) })
.OrderBy(x => x.requestTimestamp)
.ToList();
            return groups;
        }
        private DateTime UnixToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp / 1000).ToLocalTime();
            return dtDateTime;
        }
        private DataTable ExecuteQuery(string query)
        {
            string strConnection = ConfigurationManager.ConnectionStrings["oracleserver"].ToString();
            using (OracleConnection conn = new OracleConnection(strConnection))
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
        private DataTable ExecuteQueryMetric(string query)
        {
            string strConnection = ConfigurationManager.ConnectionStrings["ORMetric"].ToString();
            using (OracleConnection conn = new OracleConnection(strConnection))
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
    public class ApplicationUsage
    {
        public string ApplicationName { get; set; }
        public int HIT { get; set; }
    }

    public class Timeline
    {
        public int StatusCode { get; set; }
        public int HIT { get; set; }
        public string Time { get; set; }
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
    public class avgStatistic
    {
        public double avgservice { get; set; }
        public double avgbackend { get; set; }
        public double avgresponse { get; set; }
    }
    public class Error
    {
        public string requesttime { get; set; }
        public string applicationname { get; set; }
        public string apiname { get; set; }
        public string apiresourcepath { get; set; }
        public int responsecode { get; set; }
        public double requesttimestamp { get; set; }
    }
    public class Package
    {
        public string PackageName { get; set; }
        public int APILIMITSIZE { get; set; }
        public int APIPRICE { get; set; }
        public double OVEREXCEEDPRICE { get; set; }
    }

    public class PriceStruct
    {
        public string PackageName { get; set; }
        public string APIESBLimit { get; set; }
        public int OVERLIMIT { get; set; }
        public double OVERLIMITPRICE { get; set; }
        public double TotalPrice { get; set; }
        public int Total { get; set; }
        public double PackagePrice { get; set; }
        public double OVEREXCEEDPRICE { get; set; }
    }

    public class MetricValues
    {
        public double TIMESTAMP { get; set; }
        public double VALUE { get; set; }
    }
}
