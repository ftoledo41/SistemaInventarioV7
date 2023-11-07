using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventarioV7.Utilidades
{
    public class EmailSender : IEmailSender
    {
        //public string SendGridSecret { get; set; }

        //public EmailSender(IConfiguration _config)
        //{
        //    SendGridSecret = _config.GetValue<string>("Sendgrid:SecretKey");
        //}

        //public Task SendEmailAsync(string email, string subject, string htmlMessage)
        //{
        //    var client = new SendGridClient(SendGridSecret);
        //    var from = new EmailAddress("support@baezstone.com");
        //    var to = new EmailAddress(email);
        //    var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);

        //    return client.SendEmailAsync(msg);
        //}
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }
    }
}
