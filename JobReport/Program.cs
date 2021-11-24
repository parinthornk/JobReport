using iTextSharp.text;
using iTextSharp.text.pdf;
using jobReport.DataServices;
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
        private static string AppName_PTT = "PTT";
        private static string AppName_OR = "OR";
        private static string ConnectionString_PTT = "Data Source=10.232.108.221:1521/WSO2PRD;User Id=AM_ANALYTICS_SHARE;Password=WSO2PRD;Min Pool Size=15;Connection Lifetime=180;";
        private static string ConnectionString_OR = "Data Source=10.232.108.221:1521/PTTORWSO2PRD;User Id=AM_ANALYTICS_SHARE;Password=PTTORWSO2PRD;Min Pool Size=15;Connection Lifetime=180;";
        public static string AppName;

        private static Dictionary<string, string> SelectAppName = new Dictionary<string, string>()
        {
            { AppName_PTT, ConnectionString_PTT },
            { AppName_OR, ConnectionString_OR },
        };

        public static string GetConnectionString()
        {
            return SelectAppName[AppName];
        }

        public static bool demo = true;
        public static void Main(string[] args)
        {
            QueryManager.CalculateSessionDateTime();

            AppName = AppName_PTT;
            //QueryManager.LoadData();
            GenerateUI.CreatePdfReport();

            AppName = AppName_OR;
            //QueryManager.LoadData();
            GenerateUI.CreatePdfReport();

            //EmailManager.Send();
        }
    }
}