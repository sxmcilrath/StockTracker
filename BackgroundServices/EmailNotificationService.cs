using Microsoft.AspNetCore.Razor.TagHelpers;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace StockTracker.BackgroundServices
{
    public class EmailNotificationService
    {

        private static readonly string sg_key = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");


        public static async Task SendEmailAsync(string toEmail, string subject, string plainTextContent)
        {
            //check if sg_key is empty
            if (string.IsNullOrEmpty(sg_key))
            {
                throw new ArgumentNullException(nameof(sg_key));
            }
            var client = new SendGridClient(sg_key);
            var fromEmail = new EmailAddress("stocktracker34@gmail.com", "StockTracker");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(fromEmail, to, subject, plainTextContent, plainTextContent);

            var response = await client.SendEmailAsync(msg);
            Console.WriteLine($"Email Send Response: {response.StatusCode}");


        }

    }
}
