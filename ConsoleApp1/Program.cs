using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }


        static void Main(string[] args)
        {

            var date_begin = new DateTime(2021, 11, 17, 9, 0, 0);
            var date_end = new DateTime(2021, 11, 17, 12, 0, 0);


            List<double> stamp_list = new List<double>();
            for (int h = 9; h < 12; h++)
            {
                for (int m = 0; m < 60; m++)
                {

                    double my_long = (double)(new DateTime(2021, 11, 17, h, m, 0).Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);

                    stamp_list.Add(my_long);

                    /*
                    var x = 1000 * (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;


                    int unixTimestamp = (int)x;


                    stamp_list.Add(unixTimestamp);*/
                }
            }


            /*
             

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
             */


            stamp_list.ForEach(time =>
            {
                time = time - (time % (5000 * 60));
                Console.WriteLine(UnixTimeStampToDateTime(time / 1000));
            });






            Console.WriteLine("Heloo");
        }
    }
}
