using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobReport
{
    internal class Calculation
    {
        internal double GetUnix(DateTime date)
        {
            return (double)(date.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);
        }

        public static long date_to_unix(DateTime date)
        {
            var time = date.Ticks;
            var unix = time - new DateTime(1970, 1, 1, 0, 0, 0).Ticks;
            return long.Parse(unix.ToString().Substring(0, "1635638400000".Length));
        }

        public static DateTime unix_to_date(long unix)
        {
            // 1970, 01, 01 --> 621355968000000000
            return new DateTime(621355968000000000 + long.Parse(unix + "0000"));
        }
    }
}
