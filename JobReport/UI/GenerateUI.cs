using iTextSharp.text;
using iTextSharp.text.pdf;
using jobReport.DataServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace JobReport
{
    public class GenerateUI
    {
        private static int count_total = 0;

        private static Font GetFont(float size, int mode_bold, BaseColor color)
        {
            return FontFactory.GetFont("Century Gothic", size, mode_bold, color);
        }

        private static BaseColor ColorDarkBlue
        {
            get
            {
                return new BaseColor(47, 47, 95);
            }
        }

        private static BaseColor ColorDark
        {
            get
            {
                return new BaseColor(63, 63, 63);
            }
        }

        private static void Main_old(string[] args)
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
            if (!isVPN)
            {
                //doVPN();
            }
            Chart Chart1 = new Chart();

            DateTime dt = DateTime.Now;
            string startDate = new DateTime(dt.Year, dt.Month - 1, 1).ToString("MM/dd/yyyy");
            string endDate = new DateTime(dt.Year, dt.Month - 1, DateTime.DaysInMonth(dt.Year, dt.Month - 1)).ToString("MM/dd/yyyy");
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

        private static PdfPTable CreateTableHeader()
        {
            var font_style_col_header = GetFont(8f, Font.NORMAL, ColorDarkBlue);

            var font_style_col_body = GetFont(11f, Font.NORMAL, ColorDark);

            var table = new PdfPTable(3);
            table.SetWidths(new float[] { 100f, 100f, 50f });
            table.WidthPercentage = 100;
            table.AddCell(new PdfPCell(new Phrase(new Chunk("Report from", font_style_col_header)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("Report to", font_style_col_header)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("Package", font_style_col_header)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });

            var text_date_format = "dd/MMM/yyy";
            var text_date_begin = QueryManager.GetSessionDateTimeBegin().ToString(text_date_format, CultureInfo.CreateSpecificCulture("en-US")).ToUpper();
            var text_date_end = QueryManager.GetSessionDateTimeEnd().AddDays(-1).ToString(text_date_format, CultureInfo.CreateSpecificCulture("en-US")).ToUpper();

            table.AddCell(new PdfPCell(new Phrase(new Chunk(text_date_begin, font_style_col_body)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(text_date_end, font_style_col_body)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("Unlimited", font_style_col_body)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            return table;
        }

        private static PdfPTable CreateTableResponseTimeHead()
        {
            var font_style_col_header = GetFont(8, Font.NORMAL, ColorDarkBlue);
            var table = new PdfPTable(5);
            table.SetWidths(new float[] { 100f, 35f, 100f, 35f, 100f });
            table.WidthPercentage = 100;
            table.AddCell(new PdfPCell(new Phrase(new Chunk("AVG Service Time", font_style_col_header)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(" ", font_style_col_header)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("AVG Backend Time", font_style_col_header)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(" ", font_style_col_header)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("AVG Response Time", font_style_col_header)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            return table;
        }

        private static PdfPTable CreateTableResponseTimeBody()
        {

            // response time
            var list_avg = QueryManager.GetListAVG();

            var avg_serv = list_avg[0].AGVservice;
            var avg_back = list_avg[0].AGVbackend;
            var avg_resp = list_avg[0].AGVresponse;

            var text_avg_serv = string.Format("{0:0.00}", avg_serv) + "ms";
            var text_avg_back = string.Format("{0:0.00}", avg_back) + "ms";
            var text_avg_resp = string.Format("{0:0.00}", avg_resp) + "ms";

            var font_style_col_body = GetFont(14, Font.NORMAL, ColorDark);

            var table = new PdfPTable(5);
            table.SetWidths(new float[] { 100f, 35f, 100f, 35f, 100f });
            table.WidthPercentage = 100;
            table.AddCell(new PdfPCell(new Phrase(new Chunk(text_avg_serv, font_style_col_body)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(" ")))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(text_avg_back, font_style_col_body)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(" ")))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(text_avg_resp, font_style_col_body)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });
            return table;
        }

        private static PdfPTable CreateTableDetailHead()
        {
            var font_style = GetFont(10f, Font.NORMAL, ColorDarkBlue);

            var table = new PdfPTable(1);
            var width = new float[] { 100f };
            table.SetWidths(width);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 2;
            table.AddCell(new PdfPCell(new Phrase(new Chunk("Transaction summary by Application", font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            return table;
        }

        private static PdfPTable CreateTableDetailBody()
        {
            var font_style = GetFont(6.5f, Font.NORMAL, ColorDark);

            var table = new PdfPTable(2);
            var widths = new float[] { 80f, 40f };
            table.SetWidths(widths);
            table.WidthPercentage = 40;
            table.HorizontalAlignment = 2;

            // get queried data
            var app_count_data = QueryManager.GetListHit();

            // sort?
            app_count_data = log_API_for_Time.Sort(app_count_data);

            table.AddCell(new PdfPCell(new Phrase(new Chunk(" ", font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(" ", font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });

            count_total = 0;
            foreach (var x in app_count_data)
            {
                count_total += x.HIT;
                table.AddCell(new PdfPCell(new Phrase(new Chunk(x.ApplicationName, font_style)))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                });
                table.AddCell(new PdfPCell(new Phrase(new Chunk(string.Format("{0:n0}", x.HIT), font_style)))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                });
                table.AddCell(new PdfPCell(new Phrase(new Chunk(" ", font_style)))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                });
                table.AddCell(new PdfPCell(new Phrase(new Chunk(" ", font_style)))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                });
                table.AddCell(new PdfPCell(new Phrase(new Chunk(string.Empty, font_style)))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    BorderColor = new BaseColor(215, 215, 215),
                });
                table.AddCell(new PdfPCell(new Phrase(new Chunk(string.Empty, font_style)))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    BorderColor = new BaseColor(215, 215, 215),
                });
            }
            table.AddCell(new PdfPCell(new Phrase(new Chunk("Total", font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(string.Format("{0:n0}", count_total), font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(" ", font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(" ", font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(string.Empty, font_style)))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                BorderColor = new BaseColor(215, 215, 215),
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(string.Empty, font_style)))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                BorderColor = new BaseColor(215, 215, 215),
            });
            return table;
        }

        private static PdfPTable CreateTableSummaryHead()
        {
            var font_style = GetFont(10f, Font.NORMAL, ColorDarkBlue);

            var table = new PdfPTable(1);
            var width = new float[] { 100f };
            table.SetWidths(width);
            table.WidthPercentage = 40;
            table.HorizontalAlignment = 2;
            table.AddCell(new PdfPCell(new Phrase(new Chunk(" ", font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(" ", font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("Charging Summary", font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(" ", font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            return table;
        }

        private static PdfPTable CreateTableSummaryBody()
        {
            var font_style = GetFont(6.5f, Font.NORMAL, ColorDark);

            var table = new PdfPTable(2);
            var widths = new float[] { 80f, 40f };
            table.SetWidths(widths);
            table.WidthPercentage = 40;
            table.HorizontalAlignment = 2;

            table.AddCell(new PdfPCell(new Phrase(new Chunk("Total Usage", font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk(string.Format("{0:n0}", count_total), font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(" ", font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(" ", font_style)))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(string.Empty, font_style)))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                BorderColor = new BaseColor(215, 215, 215),
            });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(string.Empty, font_style)))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                BorderColor = new BaseColor(215, 215, 215),
            });
            return table;
        }
        
        private static Image CreateChartPie()
        {
            var data = QueryManager.GetListHit().ToArray();

            var app_names = Array.ConvertAll(data, new Converter<log_API_for_Time, string>(log =>
            {
                return log.ApplicationName;
            }));

            var app_hits = Array.ConvertAll(data, new Converter<log_API_for_Time, int>(log =>
            {
                return log.HIT;
            }));

            var width = 800;
            var height = 600;

            Chart Chart1 = new Chart();
            Chart1.Size = new System.Drawing.Size((int)width, (int)height);
            Chart1.Series.Add("Application Usage");
            Chart1.Series[0].ChartType = SeriesChartType.Pie;
            Chart1.Series[0].Points.DataBindXY(app_names, app_hits);
            Chart1.Series[0].CustomProperties = "PieLabelStyle=Outside";
            Chart1.ChartAreas.Add("ss");
            Chart1.ChartAreas[0].Area3DStyle.Enable3D = true;
            Chart1.ChartAreas[0].Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            Chart1.SaveImage(ms, ChartImageFormat.Png);
            ms.Seek(0, System.IO.SeekOrigin.Begin);
            iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(ms);
            if (pic.Height > pic.Width)
            {
                //Maximum height is 800 pixels.
                float percentage = 450 / pic.Height;
                pic.ScalePercent(percentage * 100);
            }
            else
            {
                //Maximum width is 600 pixels.
                float percentage = 350 / pic.Width;
                pic.ScalePercent(percentage * 100);
            }

            return pic;
        }

        private static Image CreateChartLine()
        {
            /*var data = QueryManager.GetListHit().ToArray();

            var app_hits = Array.ConvertAll(data, new Converter<log_API_for_Time, int>(log =>
            {
                return log.HIT;
            }));*/

            var example_dict = new Dictionary<int, int>()
            {
                { 1, 500 },
                { 2, 1500 },
                { 3, 500 },
                { 4, 2500 },
                { 5, 500 },
                { 6, 1500 },
            };

            var width = 400;
            var height = 300;
            var chart = new Chart();
            chart.Size = new System.Drawing.Size((int)width, (int)height);
            chart.Series.Add("Number of transaction over Time");
            chart.Series[0].ChartType = SeriesChartType.Line;
            var ax = example_dict.Keys.ToArray();
            var ay = example_dict.Values.ToArray();
            chart.Series[0].Points.DataBindXY(ax, ay);
            //chart.Series[0].CustomProperties = "PieLabelStyle=Outside";
            var ms = new MemoryStream();
            chart.SaveImage(ms, ChartImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            var pic = Image.GetInstance(ms);
            if (pic.Height > pic.Width)
            {
                //Maximum height is 800 pixels.
                float percentage = 450 / pic.Height;
                pic.ScalePercent(percentage * 100);
            }
            else
            {
                //Maximum width is 600 pixels.
                float percentage = 350 / pic.Width;
                pic.ScalePercent(percentage * 100);
            }
            return pic;
        }

        public static void CreatePDF()
        {
            // header table
            var table_header = CreateTableHeader();

            // response time table
            var table_response_time_head = CreateTableResponseTimeHead();
            var table_response_time_body = CreateTableResponseTimeBody();

            // tables
            var table_detail_head = CreateTableDetailHead();
            var table_detail_body = CreateTableDetailBody();
            var table_summary_head = CreateTableSummaryHead();
            var table_summary_body = CreateTableSummaryBody();

            // pie charts
            var chart_pie = CreateChartPie();

            // line chart
            var chart_line = CreateChartLine();

            // ---- table inside table ---------------------------------------------------------------- //

            // main table
            var table_complex = new PdfPTable(3);
            table_complex.SetWidths(new float[] { 65f, 5.0f, 35f, });
            table_complex.WidthPercentage = 100;

            // charts in the left
            var table_complex_left = new PdfPTable(1);
            table_complex_left.SetWidths(new float[] { 100 });
            table_complex_left.WidthPercentage = 100;
            table_complex_left.AddCell(new PdfPCell(chart_pie) { BorderWidth = 0, });
            //table_complex_left.AddCell(new PdfPCell(chart_line) { BorderWidth = 0, });
            table_complex.AddCell(new PdfPCell(table_complex_left) { BorderWidth = 0, });

            // little space in the middle
            table_complex.AddCell(new PdfPCell(new Phrase(new Chunk(" ", GetFont(5, Font.BOLD, ColorDark))))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });

            // table in the right
            var table_complex_right = new PdfPTable(1);
            table_complex_right.SetWidths(new float[] { 100 });
            table_complex_right.WidthPercentage = 100;
            table_complex_right.AddCell(new PdfPCell(table_detail_head) { BorderWidth = 0, });
            table_complex_right.AddCell(new PdfPCell(table_detail_body) { BorderWidth = 0, });
            table_complex_right.AddCell(new PdfPCell(table_summary_head) { BorderWidth = 0, });
            table_complex_right.AddCell(new PdfPCell(table_summary_body) { BorderWidth = 0, });
            table_complex.AddCell(new PdfPCell(table_complex_right) { BorderWidth = 0, });

            // ---------------------------------------------------------------------------------------- //

            // file name
            var file_name = GetPdfFileName();

            var document = new Document(PageSize.A4);
            FileStream fs = new FileStream(file_name + ".pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            PdfWriter.GetInstance(document, fs);

            document.Open();

            document.Add(new Paragraph(" "));
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph(" "));

            document.Add(table_header);

            document.Add(new Paragraph(" "));
            document.Add(new Paragraph(" "));

            document.Add(table_response_time_head);

            document.Add(new Paragraph(new Chunk(" ", new Font(Font.FontFamily.COURIER, 5f))));

            document.Add(table_response_time_body);

            document.Add(new Paragraph(" "));
            document.Add(new Paragraph(" "));

            document.Add(table_complex);

            document.Close();
        }

        public static string GetPdfFileName()
        {
            var yyyy_MM = QueryManager.GetSessionDateTimeBegin().ToString("yyyy_MM");
            return "Transaction Report " + yyyy_MM;
        }
    }
}