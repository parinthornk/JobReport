using iTextSharp.text;
using iTextSharp.text.pdf;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;

namespace JobReport
{
    public class Program
    {
        public static readonly string AppName_PTT = "PTT";
        public static readonly string AppName_OR = "OR";

        public static string AppName;
        public static string ConnectionString;

        public static int ErrorCount = 0;
        public static int RetryCount = 3;
        public static int RetryInterval = 10 * 60 * 1000;

        public static bool demo = false;

        public static void Main(string[] args)
        {
            QueryManager.CalculateSessionDateTime(DateTime.Now, args);

            GenerateReport(AppName_PTT);

            GenerateReport(AppName_OR);

            EmailManager.Send();
        }

        private static void GenerateReport(string app)
        {
            AppName = app;
            for (int i = 0; i < RetryCount; i++)
            {
                ErrorCount = 0;
                QueryManager.LoadData();
                GenerateUI.CreatePdfReport();
                if (ErrorCount == 0)
                {
                    break;
                }
                Thread.Sleep(RetryInterval);
            }
            if (ErrorCount > 0)
            {
                // report error via email
                EmailManager.ReportError();
            }
        }
    }
}