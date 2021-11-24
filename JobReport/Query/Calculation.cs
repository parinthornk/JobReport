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
    }
}
