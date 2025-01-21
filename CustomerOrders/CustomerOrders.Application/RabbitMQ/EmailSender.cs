using MimeKit;
using MailKit.Net.Smtp;
using System;

namespace CustomerOrders.Application.RabbitMQ.Interfaces
{
    public class EmailSender
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _senderEmail = "your-email@gmail.com";
        private readonly string _senderPassword = "your-email-password";

        public void SendEmail(string from, string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Your Name", from));
            message.To.Add(new MailboxAddress("Recipient Name", to));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls); 
                    client.Authenticate(_senderEmail, _senderPassword);
                    client.Send(message);
                    client.Disconnect(true);  
                }
                catch (Exception ex)
                {
                    Console.WriteLine("E-posta gönderilirken hata oluştu: " + ex.Message);
                }
            }
        }
    }
}
