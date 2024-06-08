using EcommerceAPI.Shared.Helpers.Abstractions;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;

namespace EcommerceAPI.Shared.Helpers
{
    public class SendGridEmailSender : IAppEmailSender
    {
        public SendGridEmailSender(IOptions<SendGridEmailSenderOptions> options)
        {
            Options = options.Value;
        }

        public SendGridEmailSenderOptions Options { get; set; }

        public async Task<EmailResponse> SendEmailAsync(string email, string subject, string message)
        {
            var response = await Execute(Options.ApiKey, subject, message, [email], false);
            return new() { StatusCode = response.StatusCode, Body = await response.Body.ReadAsStringAsync() };
        }

        public async Task<EmailResponse> SendEmailAsync(string[] emails, string subject, string message, bool sendAsBCC)
        {
            var response = await Execute(Options.ApiKey, subject, message, emails, sendAsBCC);
            return new() { StatusCode = response.StatusCode, Body = await response.Body.ReadAsStringAsync() };
        }

        private async Task<Response> Execute(string apiKey, string subject, string message, string[] emails, bool sendAsBCC)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(Options.SenderEmail, Options.SenderName),
                Subject = subject,
                HtmlContent = message,
            };

            if (sendAsBCC)
            {
                msg.AddTo(new EmailAddress(Options.SenderEmail));
                msg.AddBccs(emails.Select(x => new EmailAddress(x)).ToList());
            }
            else
            {
                msg.AddTos(emails.Select(x => new EmailAddress(x)).ToList());
            }

            msg.SetClickTracking(false, false);
            msg.SetOpenTracking(false);
            msg.SetGoogleAnalytics(false);
            msg.SetSubscriptionTracking(false);

            return await client.SendEmailAsync(msg);
        }
    }

    public class SendGridEmailSenderOptions
    {
        public string ApiKey { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
    }
}