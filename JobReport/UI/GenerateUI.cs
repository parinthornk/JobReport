using iTextSharp.text;
using iTextSharp.text.pdf;
using jobReport.DataServices;
using System;
using System.Collections.Generic;
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

        private static PdfPTable CreateTableDetailHead()
        {
            var font_style = new Font(Font.FontFamily.HELVETICA, 11f, Font.BOLD, new BaseColor(63, 63, 63));
            var table = new PdfPTable(1);
            var width = new float[] { 100f };
            table.SetWidths(width);
            table.WidthPercentage = 40;
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
            var font_style = new Font(Font.FontFamily.HELVETICA, 8f, Font.NORMAL, new BaseColor(63, 63, 63));
            var table = new PdfPTable(2);
            var widths = new float[] { 80f, 40f };
            table.SetWidths(widths);
            table.WidthPercentage = 40;
            table.HorizontalAlignment = 2;

            // get queried data
            var app_count_data = AppCountInfo.GetExampleData();

            // sort?
            AppCountInfo.SortArray(app_count_data);

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
                count_total += x.Count;
                table.AddCell(new PdfPCell(new Phrase(new Chunk(x.Name, font_style)))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                });
                table.AddCell(new PdfPCell(new Phrase(new Chunk(string.Format("{0:n0}", x.Count), font_style)))
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
            var font_style = new Font(Font.FontFamily.HELVETICA, 11f, Font.BOLD, new BaseColor(63, 63, 63));
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
            var font_style = new Font(Font.FontFamily.HELVETICA, 8f, Font.NORMAL, new BaseColor(63, 63, 63));
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
        
        public static void CreatePDF()
        {
            // tables
            var table_detail_head = CreateTableDetailHead();
            var table_detail_body = CreateTableDetailBody();
            var table_summary_head = CreateTableSummaryHead();
            var table_summary_body = CreateTableSummaryBody();

            var document = new Document(PageSize.A4);
            FileStream fs = new FileStream("Chapter1_Example1.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            PdfWriter.GetInstance(document, fs);
            document.Open();
            document.Add(new Paragraph("Hello World! 1"));
            document.Add(new Paragraph("Hello World! 2"));
            document.Add(new Paragraph("Hello World! 3"));
            document.Add(table_detail_head);
            document.Add(table_detail_body);
            document.Add(table_summary_head);
            document.Add(table_summary_body);
            document.Add(new Paragraph("Hello World! 4"));
            document.Add(new Paragraph("Hello World! 5"));
            document.Add(new Paragraph("Hello World! 6"));
            document.Close();
        }
    }
}