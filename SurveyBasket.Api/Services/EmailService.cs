using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using SurveyBasket.Api.Settings;

namespace SurveyBasket.Api.Services
{
    public class EmailService(IOptions<MailSettings> mailSettings ,ILogger<EmailService> logger) : IEmailSender
    {
        private readonly MailSettings _mailSettings = mailSettings.Value;
        private readonly ILogger<EmailService> _logger = logger;

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailSettings.Mail),
                Subject = subject,

            };
            message.To.Add(MailboxAddress.Parse(email));
            var builder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };
            message.Body = builder.ToMessageBody();

            using var stmp = new SmtpClient();

            
            _logger.LogInformation("Sending email to {email}", email);

            stmp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            stmp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await stmp.SendAsync(message);
            stmp.Disconnect(true);
        }
    }
}
