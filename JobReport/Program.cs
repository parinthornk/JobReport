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

        public static string AppName;
        public static string ConnectionString;

        public static bool demo = false;

        public static void Main(string[] args)
        {
            QueryManager.CalculateSessionDateTime();

            AppName = AppName_PTT;
            ConnectionString = ConfigurationManager.ConnectionStrings["connStrPTT"].ToString();
            QueryManager.LoadData();
            GenerateUI.CreatePdfReport();
            
            AppName = AppName_OR;
            ConnectionString = ConfigurationManager.ConnectionStrings["connStrOR"].ToString();
            QueryManager.LoadData();
            GenerateUI.CreatePdfReport();

            EmailManager.Send();
        }
    }
}