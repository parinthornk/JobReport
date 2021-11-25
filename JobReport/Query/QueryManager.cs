using System;
using System.Collections.Generic;
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
            Calculation Cal = new Calculation();
            Method_Query MQ = new Method_Query();

            ////----- 1
            List<log_API_for_Time> LTime = MQ.GetApplicationTimeLog(Cal.GetUnix(GetQueryTimeBegin()), Cal.GetUnix(GetQueryTimeEnd()));

            foreach (log_API_for_Time LT in LTime)
            {
                Console.WriteLine("GetApplicationTimeLog  : " + LT.ApplicationName + " , " + LT.HIT);
            }

            ////----- 2
            List<AverageTime> LGAT = MQ.GetAverageTime(Cal.GetUnix(GetQueryTimeBegin()), Cal.GetUnix(GetQueryTimeEnd()));

            foreach (AverageTime T in LGAT)
            {
                Console.WriteLine("GetAverageTime  : " + " avgservice = " + T.AGVservice + " , avgbackend = " + T.AGVbackend + " , avgresponse = " + T.AGVresponse);
            }

            ////----- 3
            List<Timestamprequest> LTP = MQ.GetTimestamprequest(Cal.GetUnix(GetQueryTimeBegin()), Cal.GetUnix(GetQueryTimeEnd()));

            /*foreach (Timestamprequest T in LTP)
            {
                Console.WriteLine("GetTimestamprequest  : " + " Requesttimestamp = " + T.Requesttimestamp + " , Responsecode = " + T.Responsecode + " , Servicetime = " + T.Servicetime);
            }*/
            Console.WriteLine("LTP.Count, " + LTP.Count);

            // these will be used in UI generation
            list_hit = new List<log_API_for_Time>(LTime);
            list_avg = new List<AverageTime>(LGAT);
            list_timestamp = new List<Timestamprequest>(LTP);
        }

        internal static void CalculateSessionDateTime()
        {
            // time begin --> the begin of the last month
            // time end   --> the end of the last month
            var now = DateTime.Now;
            time_end = new DateTime(now.Year, now.Month, 1, 0, 0, 0);
            var tmp = time_end.AddDays(-1);
            time_begin = new DateTime(tmp.Year, tmp.Month, 1, 0, 0, 0);

            // use 23:59:59.999 on the last day of month
            time_end = new DateTime(time_end.Ticks - 1);

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

        private static List<Timestamprequest> list_timestamp;
        public static List<Timestamprequest> GetListTimeStamp()
        {
            if (Program.demo)
            {
                if (list_timestamp == null)
                {
                    var read = System.IO.File.ReadAllLines("time_stamp.txt");
                    list_timestamp = new List<Timestamprequest>();
                    foreach (var line in read)
                    {
                        var sep = line.Split(new string[] { ", " }, StringSplitOptions.None);
                        list_timestamp.Add(new Timestamprequest()
                        {
                            Requesttimestamp = sep[0],
                            Responsecode = int.Parse(sep[1]),
                            Servicetime = int.Parse(sep[2]),
                        });
                    }
                }
            }

            return list_timestamp;
        }
    }
}