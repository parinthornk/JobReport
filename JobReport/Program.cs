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
using System.Threading;
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

        private static double GetUnix(DateTime date)
        {
            return (double)(date.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);
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

        private static void TestOracle()
        {

            //using connection string attributes to connect to Oracle Database
            OracleConnection con = new OracleConnection();
            con.ConnectionString = "Data Source=10.232.108.221:1521/WSO2PRD;User Id=AM_ANALYTICS_SHARE;Password=WSO2PRD;Min Pool Size=15;Connection Lifetime=180;"; ;
            con.Open();
            Console.WriteLine("Connected to Oracle" + con.ServerVersion);

            var query_1 = "SELECT ApplicationDisplayName as ApplicationName, COUNT(ApplicationDisplayName) as HIT FROM APIRequestLog INNER JOIN cfg_applicationdisplayname on cfg_applicationdisplayname.applicationname=apirequestlog.applicationname and apirequestlog.applicationowner = cfg_applicationdisplayname.applicationowner WHERE requesttimestamp between {0:00} and {1:00}  GROUP bY ApplicationDisplayName";
            var query_2 = "SELECT ROUND(AVG(SERVICETIME),2)as avgservice,ROUND(AVG(Backendtime),2) as avgbackend,ROUND(AVG(Responsetime),2)as avgresponse FROM APIRequestLog WHERE requesttimestamp between {0:00} and {1:00}";
            var query_3 = "SELECT requesttimestamp,responsecode,servicetime FROM apirequestlog WHERE requesttimestamp between {0:00} and {1:00}";

            string cmdQuery = query_1;

            var date = DateTime.Now;
            date = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            date_end = date.AddDays(-date.Day);
            date_begin = date_end.AddDays(-date_end.Day + 1);

            double unix_start = GetUnix(date_begin);
            double unix_stop = GetUnix(date_end);
            cmdQuery = string.Format(cmdQuery, unix_start, unix_stop);
            Console.WriteLine(cmdQuery);

            // Create the OracleCommand
            OracleCommand cmd = new OracleCommand(cmdQuery);

            cmd.Connection = con;
            //cmd.CommandType = CommandType.Text;

            // Execute command, create OracleDataReader object
            OracleDataReader reader = cmd.ExecuteReader();

            var filed_count = reader.FieldCount;

            var list = new List<string>();

            while (reader.Read())
            {
                var text = string.Empty;
                for (int i = 0; i < filed_count; i++)
                {
                    text = text + reader.GetString(i) + ", ";
                }
                Console.WriteLine(text);

                list.Add(text);
            }

            File.WriteAllLines("list.txt", list);

            // Clean up
            reader.Dispose();
            cmd.Dispose();

            // Close and Dispose OracleConnection object
            con.Close();
            con.Dispose();
            Console.WriteLine("Disconnected");
        }

        public static void Main(string[] args)
        {
            /*var date_ref = new DateTime(1970, 1, 1);
            for (; ; )
            {
                Console.WriteLine(DateTime.Now.Millisecond);
                Thread.Sleep(1000);
            }*/

            //TestOracle();
            //GetDateBeginAndEnd();

            ReportPDF();
        }

        private static DateTime date_begin, date_end;
        private static void GetDateBeginAndEnd()
        {
            var date = DateTime.Now;
            date = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            date_end = date.AddDays(-date.Day);
            date_begin = date_end.AddDays(-date_end.Day + 1);

            double unix_start = GetUnix(date_begin);
            double unix_stop = GetUnix(date_end);
            string cmdQuery = "SELECT ApplicationDisplayName as ApplicationName, COUNT(ApplicationDisplayName) as HIT FROM APIRequestLog INNER JOIN cfg_applicationdisplayname on cfg_applicationdisplayname.applicationname=apirequestlog.applicationname and apirequestlog.applicationowner = cfg_applicationdisplayname.applicationowner WHERE requesttimestamp between {0:00} and {1:00}  GROUP bY ApplicationDisplayName";
            cmdQuery = string.Format(cmdQuery, unix_start, unix_stop);
            Console.WriteLine(cmdQuery);
        }

        private static void ReportPDF()
        {
            // create head table
            var head_font_style = new Font(Font.FontFamily.HELVETICA, 11f, Font.BOLD, new BaseColor(63, 63, 63));
            var head_table = new PdfPTable(1);
            var head_width = new float[] { 100f };
            head_table.SetWidths(head_width);
            head_table.WidthPercentage = 40;
            head_table.HorizontalAlignment = 2;
            head_table.AddCell(new PdfPCell(new Phrase(new Chunk("Transaction summary by Application", head_font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });

            // create a table
            var font_style = new Font(Font.FontFamily.HELVETICA, 8f, Font.NORMAL, new BaseColor(63, 63, 63));
            var table_app_count = new PdfPTable(2);
            var tbwidths = new float[] { 80f, 40f };
            table_app_count.SetWidths(tbwidths);
            table_app_count.WidthPercentage = 40;

            table_app_count.HorizontalAlignment = 2;

            var app_count_data = AppCountInfo.GetData();

            // sort?
            AppCountInfo.SortArray(app_count_data);

            table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk(" ", font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });
            table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk(" ", font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });

            var count_total = 0;

            foreach (var x in app_count_data)
            {
                count_total += x.Count;
                table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk(x.Name, font_style)))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                });
                table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk(string.Format("{0:n0}", x.Count), font_style)))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                });
                table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk(" ", font_style)))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                });
                table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk(" ", font_style)))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                });
                table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk(string.Empty, font_style)))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    BorderColor = new BaseColor(215, 215, 215),
                });
                table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk(string.Empty, font_style)))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    BorderColor = new BaseColor(215, 215, 215),
                });
            }


            table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk("Total", font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk(string.Format("{0:n0}", count_total), font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });
            table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk(" ", font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });
            table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk(" ", font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });
            table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk(string.Empty, font_style)))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                BorderColor = new BaseColor(215, 215, 215),
            });
            table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk(string.Empty, font_style)))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                BorderColor = new BaseColor(215, 215, 215),
            });

            // create foot table
            var foot_font_style = new Font(Font.FontFamily.HELVETICA, 11f, Font.BOLD, new BaseColor(63, 63, 63));
            var foot_table = new PdfPTable(1);
            var foot_width = new float[] { 100f };
            foot_table.SetWidths(foot_width);
            foot_table.WidthPercentage = 40;
            foot_table.HorizontalAlignment = 2;
            foot_table.AddCell(new PdfPCell(new Phrase(new Chunk(" ", foot_font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            foot_table.AddCell(new PdfPCell(new Phrase(new Chunk(" ", foot_font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            foot_table.AddCell(new PdfPCell(new Phrase(new Chunk("Charging Summary", foot_font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            foot_table.AddCell(new PdfPCell(new Phrase(new Chunk(" ", foot_font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });

            var foot_table_font_style = new Font(Font.FontFamily.HELVETICA, 8f, Font.NORMAL, new BaseColor(63, 63, 63));
            var foot_table_app_count = new PdfPTable(2);
            var foot_tbwidths = new float[] { 80f, 40f };
            foot_table_app_count.SetWidths(foot_tbwidths);
            foot_table_app_count.WidthPercentage = 40;
            foot_table_app_count.HorizontalAlignment = 2;

            foot_table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk("Total Usage", foot_table_font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });

            foot_table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk(string.Format("{0:n0}", count_total), foot_table_font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });
            foot_table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk(" ", foot_table_font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });
            foot_table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk(" ", foot_table_font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });
            foot_table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk(string.Empty, foot_table_font_style)))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                BorderColor = new BaseColor(215, 215, 215),
            });
            foot_table_app_count.AddCell(new PdfPCell(new Phrase(new Chunk(string.Empty, foot_table_font_style)))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                BorderColor = new BaseColor(215, 215, 215),
            });



            // write PDF file
            var document = new Document(PageSize.A4);
            FileStream fs = new FileStream("Chapter1_Example1.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            PdfWriter.GetInstance(document, fs);
            document.Open();
            document.Add(new Paragraph("Hello World! 1"));
            document.Add(new Paragraph("Hello World! 2"));
            document.Add(new Paragraph("Hello World! 3"));
            document.Add(head_table);
            document.Add(table_app_count);
            document.Add(foot_table);
            document.Add(foot_table_app_count);
            document.Add(new Paragraph("Hello World! 4"));
            document.Add(new Paragraph("Hello World! 5"));
            document.Add(new Paragraph("Hello World! 6"));
            document.Close();
        }
    }
}