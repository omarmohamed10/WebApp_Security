using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

namespace WebApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;
        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task SendAsync(string from , string to , string subject , string body)
        {
            var message = new MailMessage(from, to, subject, body);
            string Password = configuration["MAIL_PASSWORD"];
            string SmtpServer = configuration["SmtpServer"];
            int SmtpPort = Convert.ToInt32(configuration["SmtpPort"]);

            using (var emailClient = new SmtpClient(SmtpServer, SmtpPort))
            {
                emailClient.Credentials = new NetworkCredential(from, Password);
                await emailClient.SendMailAsync(message);
            }
        }
    }
}
