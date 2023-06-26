using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace StationManager.Application.Services
{
    public interface IMailService
    {
        bool SendMail(MailContent content);
    }

    public class MailContent
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class MailService : IMailService
    {
        private readonly IConfiguration _config;

        public MailService(IConfiguration config)
        {
            _config = config;
        }

        public bool SendMail(MailContent content)
        {
            var email = _config.GetSection("MailService")["Email"];
            var password = _config.GetSection("MailService")["Password"];

            MailMessage msg = new MailMessage();

            msg.From = new MailAddress(email);
            msg.To.Add(content.To);
            msg.Subject = content.Subject;
            msg.Body = content.Body;
            //msg.Priority = MailPriority.High;


            using (SmtpClient client = new SmtpClient())
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(email, password);
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                client.Send(msg);
            }

            return true;
        }
    }
}
