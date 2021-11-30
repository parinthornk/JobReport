using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobReport
{
    public class QueryManager
    {
        private static DateTime time_end, time_begin, time_query_begin, time_query_end;

        public static void LoadData()
        {
            Program.ConnectionString = ConfigurationManager.ConnectionStrings["connStr" + Program.AppName].ToString();
            if (!Program.demo)
            {
                Calculation Cal = new Calculation();
                Method_Query MQ = new Method_Query();

                List<log_API_for_Time> LTime = new List<log_API_for_Time>();
                List<AverageTime> LGAT = new List<AverageTime>();
                List<ResponseHit> LTP = new List<ResponseHit>();
                double PG = 0;

                ////----- 1
                try
                {
                    LTime = MQ.GetApplicationTimeLog(Cal.GetUnix(GetQueryTimeBegin()), Cal.GetUnix(GetQueryTimeEnd()));
                }
                catch
                {
                    LTime = new List<log_API_for_Time>();
                }

                foreach (log_API_for_Time LT in LTime)
                {
                    Console.WriteLine("GetApplicationTimeLog  : " + LT.ApplicationName + " , " + LT.HIT);
                }

                ////----- 2
                try
                {
                    LGAT = MQ.GetAverageTime(Cal.GetUnix(GetQueryTimeBegin()), Cal.GetUnix(GetQueryTimeEnd()));
                }
                catch
                {
                    LGAT = new List<AverageTime>();
                }

                foreach (AverageTime T in LGAT)
                {
                    Console.WriteLine("GetAverageTime  : " + " avgservice = " + T.AGVservice + " , avgbackend = " + T.AGVbackend + " , avgresponse = " + T.AGVresponse);
                }

                ////----- 3
                try
                {
                    LTP = MQ.GetTimestamprequest5minut(Cal.GetUnix(GetQueryTimeBegin()), Cal.GetUnix(GetQueryTimeEnd()));
                }
                catch
                {
                    LTP = new List<ResponseHit>();
                }

                Console.WriteLine("LTP.Count, " + LTP.Count);

                ////----- 4
                try
                {
                    PG = MQ.GetTotalCost(GetQueryTimeBegin().Month.ToString());
                }
                catch
                {
                    PG = 0;
                }

                Console.WriteLine("GetTotalCost  : " + PG);

                // these will be used in UI generation
                list_hit = new List<log_API_for_Time>(LTime);
                list_avg = new List<AverageTime>(LGAT);
                list_timestamp = new List<ResponseHit>(LTP);
                total_cost = PG;
            }
        }

        internal static void CalculateSessionDateTime(DateTime now, string[] arguments)
        {
            var dates = SessionTime.GetSessionRange(now, arguments);

            time_begin = dates[0];
            time_end = dates[1];

            time_query_begin = time_begin.ToUniversalTime();
            time_query_end = time_end.ToUniversalTime();
        }

        internal static DateTime GetSessionDateTimeBegin()
        {
            return time_begin;
        }

        internal static DateTime GetSessionDateTimeEnd()
        {
            return time_end;
        }

        internal static DateTime GetQueryTimeBegin()
        {
            return time_query_begin;
        }

        internal static DateTime GetQueryTimeEnd()
        {
            return time_query_end;
        }

        private static List<log_API_for_Time> list_hit;
        public static List<log_API_for_Time> GetListHit()
        {
            if (Program.demo)
            {
                // demo data
                list_hit = new List<log_API_for_Time>()
                {
                    new log_API_for_Time() { ApplicationName = "PTT Smart Procurement", HIT = 1776335, },
                    new log_API_for_Time() { ApplicationName = "PTT Mercury", HIT = 26, },
                    new log_API_for_Time() { ApplicationName = "PTT Vendor Management", HIT = 4743, },
                    new log_API_for_Time() { ApplicationName = "PTT SDS", HIT = 4, },
                    new log_API_for_Time() { ApplicationName = "PTT ICTCM", HIT = 2524, },
                    new log_API_for_Time() { ApplicationName = "PTT NGV Barcode", HIT = 649, },
                    new log_API_for_Time() { ApplicationName = "PTT GSM", HIT = 36, },
                    new log_API_for_Time() { ApplicationName = "PTT Receipt Recording System", HIT = 24484, },
                    new log_API_for_Time() { ApplicationName = "PTT Credit Bureau Database", HIT = 89199, },
                    new log_API_for_Time() { ApplicationName = "PTT Electronic Bank Guarantee", HIT = 316769, },
                    new log_API_for_Time() { ApplicationName = "PTT Standard Price", HIT = 16, },
                };
            }

            return list_hit;
        }

        private static List<AverageTime> list_avg;
        public static List<AverageTime> GetListAVG()
        {
            if (Program.demo)
            {
                // demo data
                list_avg = new List<AverageTime>()
                {
                    new AverageTime(){ AGVservice = 8.4, AGVbackend = 430.05, AGVresponse = 438.45 }
                };
            }

            return list_avg;
        }

        private static double total_cost;
        public static double GetTotalCost()
        {
            if (Program.demo)
            {
                total_cost = 416666.67;
            }
            return total_cost;
        }

        private static List<ResponseHit> list_timestamp;
        public static List<ResponseHit> GetListTimeStamp()
        {
            if (Program.demo)
            {
                if (list_timestamp == null)
                {
                    var read = System.IO.File.ReadAllLines("GetTimestamprequest5minut.txt");
                    list_timestamp = new List<ResponseHit>();
                    foreach (var line in read)
                    {
                        var sep = line.Split(new string[] { "|" }, StringSplitOptions.None);
                        list_timestamp.Add(new ResponseHit()
                        {
                            requestTimestamp = double.Parse(sep[2]),
                            HIT = int.Parse(sep[1]),
                            AvgServiceTime = sep[0],
                        });
                    }
                }
            }

            return list_timestamp;
        }
    }
}