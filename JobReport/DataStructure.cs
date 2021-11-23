using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobReport
{
    public class AppCountInfo
    {
        private class Sort : IComparer<AppCountInfo>
        {
            public int Compare(AppCountInfo x, AppCountInfo y)
            {
                if (x.Count > y.Count)
                {
                    return -1;
                }
                if (x.Count < y.Count)
                {
                    return +1;
                }
                return 0;
            }
        }

        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }

        public static AppCountInfo[] GetData()
        {
            return new AppCountInfo[]
            {
                new AppCountInfo() { Name = "PTT Smart Procurement", Count = 1776335, },
                new AppCountInfo() { Name = "PTT Mercury", Count = 26, },
                new AppCountInfo() { Name = "PTT Vendor Management", Count = 4743, },
                new AppCountInfo() { Name = "PTT SDS", Count = 4, },
                new AppCountInfo() { Name = "PTT ICTCM", Count = 2524, },
                new AppCountInfo() { Name = "PTT NGV Barcode", Count = 649, },
                new AppCountInfo() { Name = "PTT GSM", Count = 36, },
                new AppCountInfo() { Name = "PTT Receipt Recording System", Count = 24484, },
                new AppCountInfo() { Name = "PTT Credit Bureau Database", Count = 89199, },
                new AppCountInfo() { Name = "PTT Electronic Bank Guarantee", Count = 316769, },
                new AppCountInfo() { Name = "PTT Standard Price", Count = 16, },
            };
        }

        public static void SortArray(AppCountInfo[] app_count_data)
        {
            Array.Sort(app_count_data, new Sort());
        }
    }
}