using iTextSharp.text;
using iTextSharp.text.pdf;
using jobReport.DataServices;
using Oracle.ManagedDataAccess.Client;
//using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace JobReport
{
    class Program
    {

        private static string ConnString = "Data Source=10.232.108.221:1521/WSO2PRD;User Id=AM_ANALYTICS_SHARE;Password=WSO2PRD;Min Pool Size=15;Connection Lifetime=180;";
        private static string QueryString = "SELECT * FROM cfg_ConsumerPackage where packagename='{0}'"; // <----

        static void Main_old(string[] args)
        {
            bool isVPN = false;
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface Interface in interfaces)
                {
                    if (Interface.OperationalStatus == OperationalStatus.Up && Interface.Description.Contains("Cisco"))
                    {
                        isVPN = true;
                        break;
                    }
                }
            }
            if (!isVPN) {
                //doVPN();
            }
            Chart Chart1 = new Chart();

            DateTime dt = DateTime.Now;
            string startDate = new DateTime(dt.Year, dt.Month - 1, 1).ToString("MM/dd/yyyy");
            string endDate = new DateTime(dt.Year, dt.Month - 1, DateTime.DaysInMonth(dt.Year,dt.Month-1)).ToString("MM/dd/yyyy");
            var z = new AnalyticsData().GetApplicationUsage(startDate, endDate);
            //var z = new AnalyticsData().GetApplicationUsage("10/1/2021", "10/31/2021");
            Chart1.Size = new System.Drawing.Size(800, 600);
            string[] x = (from p in z
                          orderby p.HIT ascending
                          select p.ApplicationName).ToArray();

            //Get the Total of Orders for each City.
            int[] y = (from p in z
                       orderby p.HIT ascending
                       select p.HIT).ToArray();
            Chart1.Series.Add("Application Usage");
            Chart1.Series[0].ChartType = SeriesChartType.Pie;
            Chart1.Series[0].Points.DataBindXY(x, y);
            Chart1.Series[0].CustomProperties = "PieLabelStyle=Outside";
            Chart1.ChartAreas.Add("ss");
            Chart1.ChartAreas[0].Area3DStyle.Enable3D = true;
            Chart1.ChartAreas[0].Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            Chart1.SaveImage(ms, ChartImageFormat.Jpeg);
            ms.Seek(0, System.IO.SeekOrigin.Begin);
            iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(ms);
                if (pic.Height > pic.Width)
                {
                    //Maximum height is 800 pixels.
                    float percentage = 0.0f;
                    percentage = 450 / pic.Height;
                    pic.ScalePercent(percentage * 100);
                }
                else
                {
                    //Maximum width is 600 pixels.
                    float percentage = 0.0f;
                    percentage = 350 / pic.Width;
                    pic.ScalePercent(percentage * 100);
                }
            Font bold = new Font(Font.FontFamily.COURIER, 9f, Font.NORMAL, new BaseColor(163, 21, 21));
            PdfPTable PdfTable = new PdfPTable(3);//#1
            float[] tbwidths = { 50f, 50f, 50f };
            PdfTable.SetWidths(tbwidths);
            PdfTable.WidthPercentage = 100;
            PdfPCell PdfCell = null;
            PdfCell = new PdfPCell(new Phrase(new Chunk("1", bold)));//#2
            PdfCell.Border = 0;
            PdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            PdfTable.AddCell(PdfCell);
            PdfCell = new PdfPCell(new Phrase(new Chunk("2", bold)));//#3
            PdfCell.Border = 0;
            PdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            PdfTable.AddCell(PdfCell);
            PdfCell = new PdfPCell(new Phrase(new Chunk("3", bold)));//#4
            PdfCell.Border = 0;
            PdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            PdfTable.AddCell(PdfCell);
            Document document = new Document(PageSize.A4);
            FileStream fs = new FileStream("Chapter1_Example1.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            PdfWriter.GetInstance(document, fs);
            document.Open();
            document.Add(PdfTable);
            document.Add(pic);
            document.Close();


            //smtp sendmail boripan.c@,tawit.c@,arisa.p@ ,zpanuwat.p@,zphumet.a@,parinthorn.k@
        }

        private static void TestMod()
        {


            var bold = new Font(Font.FontFamily.COURIER, 9f, Font.NORMAL, new BaseColor(163, 21, 21));
            var PdfTable = new PdfPTable(3);
            var tbwidths = new float[] { 50f, 50f, 50f };
            PdfTable.SetWidths(tbwidths);
            PdfTable.WidthPercentage = 100;
            PdfTable.AddCell(new PdfPCell(new Phrase(new Chunk("1", bold)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            PdfTable.AddCell(new PdfPCell(new Phrase(new Chunk("22", bold)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            PdfTable.AddCell(new PdfPCell(new Phrase(new Chunk("333", bold)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_CENTER
            });




            // write PDF file
            var document = new Document(PageSize.A4);
            FileStream fs = new FileStream("Chapter1_Example1.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            PdfWriter.GetInstance(document, fs);
            document.Open();
            document.Add(PdfTable);
            document.Close();
        }

        private static double GetUnix(DateTime date)
        {
            return (double)(date.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);
        }

        public static void Main(string[] args)
        {


            OracleConnection con = new OracleConnection();

            //using connection string attributes to connect to Oracle Database
            con.ConnectionString = "Data Source=10.232.108.221:1521/WSO2PRD;User Id=AM_ANALYTICS_SHARE;Password=WSO2PRD;Min Pool Size=15;Connection Lifetime=180;"; ;
            con.Open();
            Console.WriteLine("Connected to Oracle" + con.ServerVersion);





            double unix_start = GetUnix(DateTime.Now.AddDays(-1));

            double unix_stop = GetUnix(DateTime.Now);


            string cmdQuery = "SELECT ApplicationDisplayName as ApplicationName, COUNT(ApplicationDisplayName) as HIT FROM APIRequestLog INNER JOIN cfg_applicationdisplayname on cfg_applicationdisplayname.applicationname=apirequestlog.applicationname and apirequestlog.applicationowner = cfg_applicationdisplayname.applicationowner WHERE requesttimestamp between {0:00} and {1:00}  GROUP bY ApplicationDisplayName";

            cmdQuery = string.Format(cmdQuery, unix_start, unix_stop);


            // Create the OracleCommand
            OracleCommand cmd = new OracleCommand(cmdQuery);

            cmd.Connection = con;
            //cmd.CommandType = CommandType.Text;

            // Execute command, create OracleDataReader object
            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                // output Employee Name and Number
                Console.WriteLine("Employee Name : " + reader.GetString(0) + " , ");
                  //"Employee Number : " + reader.GetDecimal(1));
            }

            // Clean up
            reader.Dispose();
            cmd.Dispose();





            // Close and Dispose OracleConnection object
            con.Close();
            con.Dispose();
            Console.WriteLine("Disconnected");


        }



        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }
    }
}