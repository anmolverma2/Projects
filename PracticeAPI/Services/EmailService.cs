using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PracticeAPI.Model;

namespace PracticeAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSetting emailSetting;
        public EmailService(IOptions<EmailSetting> options)
        {
            this.emailSetting = options.Value;
        }
        public async Task SendEmailAsync(MailRequest request)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailSetting.Email);
            email.To.Add(MailboxAddress.Parse(request.ToEmail));
            email.Subject = request.Subject;

            var builder = new BodyBuilder();

            /* Code for Attachment */
            byte[] filebytes;
            if (System.IO.File.Exists("Attachment/Anmol CV.pdf"))
            {
                FileStream file = new FileStream("Attachment/Anmol CV.pdf", FileMode.Open, FileAccess.Read);
                using(MemoryStream  ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    filebytes = ms.ToArray();
                }
                builder.Attachments.Add("Anmol_CV.pdf", filebytes, ContentType.Parse("application/octet-stream"));
               // for multiple attachment
                // builder.Attachments.Add("Anmol_CV.pdf", filebytes, ContentType.Parse("application/octet-stream"));
            }

            builder.HtmlBody = request.Body;
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(emailSetting.Host, emailSetting.Port,SecureSocketOptions.StartTls);
            smtp.Authenticate(emailSetting.Email,emailSetting.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
            

        }
    }
}
