using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using FluentEmail.Core;
using FluentEmail.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace StockTracker.BackgroundServices
{
    public static class FluentEmailExtensions
    {
        public static void AddFluentEmail(this IServiceCollection services, ConfigurationManager configuration)
        {
            var emailSettings = configuration.GetSection("EmailSettings");

            var defaultFromEmail = emailSettings["DefaultFromEmail"];
            var host = emailSettings["SMTPSetting:Host"];
            var port = emailSettings.GetValue<int>("Port");
            var userName = emailSettings["UserName"];
            var password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");

            services.AddFluentEmail(defaultFromEmail)
                .AddSmtpSender(new SmtpClient(host)
                {
                    Port = port,
                    Credentials = new System.Net.NetworkCredential(userName, password),
                    EnableSsl = false
                });
            

        }


    }
}
