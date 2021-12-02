using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace JobReport
{
    public class EmailManager
    {
        public static void Send()
        {
            if (Program.demo)
            {
                return;
            }

            var email = ConfigurationManager.AppSettings["email"];
            var email_to = ConfigurationManager.AppSettings["email_to"];
            var email_server = ConfigurationManager.AppSettings["email_server"];
            var email_password = ConfigurationManager.AppSettings["email_password"];
            var email_port = ConfigurationManager.AppSettings["email_port"];
            var email_ssl_enable = ConfigurationManager.AppSettings["email_ssl_enable"];

            var date_begin = QueryManager.GetSessionDateTimeBegin().ToString("yyyy-MM-dd");
            var date_end = QueryManager.GetSessionDateTimeEnd().ToString("yyyy-MM-dd");

            using (MailMessage mm = new MailMessage(email, email_to))
            {
                mm.Subject = "WSO2 Transaction Report";
                mm.Body = $"WSO2 transaction report from {date_begin} to {date_end}.";

                mm.Attachments.Add(new Attachment(GenerateUI.GetPdfFileName(Program.AppName_PTT) + ".pdf"));
                mm.Attachments.Add(new Attachment(GenerateUI.GetPdfFileName(Program.AppName_OR) + ".pdf"));

                mm.IsBodyHtml = false;
                var smtp = new SmtpClient();
                smtp.Host = email_server;
                smtp.EnableSsl = email_ssl_enable.ToLower() == "true";
                NetworkCredential NetworkCred = new NetworkCredential(email.Trim(), email_password.Trim());
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = int.Parse(email_port);
                smtp.Send(mm);
            }

            File.WriteAllText("email_sent_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt", string.Empty);
        }

        internal static void ReportError()
        {
            try
            {
                var email = ConfigurationManager.AppSettings["email"];
                var email_to = ConfigurationManager.AppSettings["email_to"];
                var email_server = ConfigurationManager.AppSettings["email_server"];
                var email_password = ConfigurationManager.AppSettings["email_password"];
                var email_port = ConfigurationManager.AppSettings["email_port"];
                var email_ssl_enable = ConfigurationManager.AppSettings["email_ssl_enable"];

                using (MailMessage mm = new MailMessage(email, email_to))
                {
                    mm.Subject = "Error Connect to Oracle DB";
                    mm.Body = $"Failed to connect to Oracle DB, " + DateTime.Now.ToString();

                    mm.IsBodyHtml = false;
                    var smtp = new SmtpClient();
                    smtp.Host = email_server;
                    smtp.EnableSsl = email_ssl_enable.ToLower() == "true";
                    NetworkCredential NetworkCred = new NetworkCredential(email.Trim(), email_password.Trim());
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = int.Parse(email_port);
                    smtp.Send(mm);
                }
            }
            catch
            {

            }
        }
    }
}