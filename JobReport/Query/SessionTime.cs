using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobReport
{
    public class SessionTime
    {
        public const string TimeFormat = "yyyy-MM-dd-HH-mm-ss";

        public static DateTime[] GetSessionRange(DateTime now, string[] arguments)
        {
            if (arguments == null)
            {
                // use one month default
                return GetSessionDefault(now);
            }

            else if (arguments.Length == 2)
            {
                // use custom range
                return GetSessionByArgs(now, arguments);
            }

            else
            {
                // use one month default
                return GetSessionDefault(now);
            }
        }

        private static DateTime[] GetSessionDefault(DateTime now)
        {
            var d_1 = now.AddDays(1 - now.Day);
            d_1 = new DateTime(d_1.Year, d_1.Month, d_1.Day, 0, 0, 0);
            var time_end = new DateTime(d_1.Ticks - 1);
            var d_0 = d_1.AddDays(-1);
            var time_begin = d_0.AddDays(1 - d_0.Day);
            return new DateTime[] { time_begin, time_end };
        }

        private static DateTime[] GetSessionByArgs(DateTime now, string[] arguments)
        {
            try
            {
                var time_begin = DateTime.ParseExact(arguments[0], TimeFormat, null);
                var time_end = DateTime.ParseExact(arguments[1], TimeFormat, null);
                var cmp = DateTime.Compare(time_begin, time_end);
                if (cmp > 0)
                {
                    throw new Exception("Invalid session date.");
                }
                return new DateTime[] { time_begin, time_end };
            }
            catch
            {
                return GetSessionDefault(now);
            }
        }
    }
}