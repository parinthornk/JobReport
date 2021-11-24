using System;
using System.Collections.Generic;
using System.Configuration;
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
            var email = ConfigurationManager.AppSettings["email"];
            var email_to = ConfigurationManager.AppSettings["email_to"];
            var email_server = ConfigurationManager.AppSettings["email_server"];
            var email_password = ConfigurationManager.AppSettings["email_password"];

            var file_name = GenerateUI.GetPdfFileName();
            var date_begin = QueryManager.GetSessionDateTimeBegin().ToString("yyyy-MM-dd");
            var date_end = QueryManager.GetSessionDateTimeEnd().AddDays(-1).ToString("yyyy-MM-dd");

            using (MailMessage mm = new MailMessage(email, email_to))
            {
                mm.Subject = file_name;
                mm.Body = $"WSO2 transaction report from {date_begin} to {date_end}.";
                System.Net.Mail.Attachment attachment;
                attachment = new Attachment(file_name + ".pdf");
                mm.Attachments.Add(attachment);
                mm.IsBodyHtml = false;
                var smtp = new SmtpClient();
                smtp.Host = email_server;
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential(email, email_password);
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
        }
    }
}