using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace MDM.WebPortal.Tools
{
    public class Mail
    {
        // constants

        private const string HtmlEmailHeader = "<html><head><title></title></head><body style='font-family:arial; font-size:14px;'>";
        private const string HtmlEmailFooter = "";

        // properties

        public List<string> To { get; set; }
        public List<string> CC { get; set; }
        public List<string> BCC { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        // constructor
        public Mail()
        {
            To = new List<string>();
            CC = new List<string>();
            BCC = new List<string>();
        }

        // send
        public Task SendAsync()
        {
            // Credentials:
            var sendGridUserName = "do_not_reply@medprosystems.net";
            var sentFrom = "do_not_reply@medprosystems.net";
            var sendGridPassword = "MEDpro15!";

            // Configure the client:
            var client =
                new System.Net.Mail.SmtpClient("smtp.office365.com", Convert.ToInt32(587));

            client.Port = 587;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            // Creatte the credentials:
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential(sendGridUserName, sendGridPassword);

            client.EnableSsl = true;
            client.Credentials = credentials;

            MailMessage message = new MailMessage();

            foreach (var x in To)
            {
                message.To.Add(x);
            }
            foreach (var x in CC)
            {
                message.CC.Add(x);
            }
            foreach (var x in BCC)
            {
                message.Bcc.Add(x);
            }

            message.Subject = Subject;
            message.Body = Body;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.From = new MailAddress(sentFrom);
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;



            return client.SendMailAsync(message);
        }
    }
}