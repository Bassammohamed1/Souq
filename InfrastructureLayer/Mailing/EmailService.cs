using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace InfrastructureLayer.Mailing
{
    public class EmailService : IEmailSender
    {
        private readonly MailSettings _settings;

        public EmailService(IOptions<MailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_settings.UserName, _settings.DisplayName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mail.To.Add(email);

            using var smtp = new SmtpClient(_settings.smtp, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.UserName, _settings.Password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mail);
        }
    }
}
