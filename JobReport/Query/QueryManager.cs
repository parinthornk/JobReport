using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobReport
{
    public class QueryManager
    {
        public static void LoadData()
        {
            Calculation Cal = new Calculation();
            Method_Query MQ = new Method_Query();

            ////----- 1
            List<log_API_for_Time> LTime = MQ.GetApplicationTimeLog(Cal.GetUnix(DateTime.Now.AddDays(-1)), Cal.GetUnix(DateTime.Now));

            foreach (log_API_for_Time LT in LTime)
            {
                Console.WriteLine("GetApplicationTimeLog  : " + LT.ApplicationName + " , " + LT.HIT);
            }
            ////----- 2
            List<AverageTime> LGAT = MQ.GetAverageTime(Cal.GetUnix(DateTime.Now.AddDays(-1)), Cal.GetUnix(DateTime.Now));

            foreach (AverageTime T in LGAT)
            {
                Console.WriteLine("GetAverageTime  : " + " avgservice = " + T.AGVservice + " , avgbackend = " + T.AGVbackend + " , avgresponse = " + T.AGVresponse);
            }
            ////----- 3
            List<Timestamprequest> LTP = MQ.GetTimestamprequest(Cal.GetUnix(DateTime.Now.AddDays(-1)), Cal.GetUnix(DateTime.Now));

            foreach (Timestamprequest T in LTP)
            {
                Console.WriteLine("GetTimestamprequest  : " + " Requesttimestamp = " + T.Requesttimestamp + " , Responsecode = " + T.Responsecode + " , Servicetime = " + T.Servicetime);
            }
        }
    }
}