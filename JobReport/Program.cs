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
        public static bool demo = true;
        public static void Main(string[] args)
        {
            // phupha, query
            QueryManager.CalculateSessionDateTime();
            if (!demo)
            {
                QueryManager.LoadData();
            }

            // mod, create PDF
            GenerateUI.CreatePDF();

            // phupha, send mail
            EmailManager.Send();
        }
    }
}