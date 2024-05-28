//using Microsoft.Extensions.Options;

//namespace EcommerceAPI.Shared.Helpers
//{
//    public interface IEmailSender
//    {
//        public Task<Response> SendEmailAsync(string email, string subject, string message);
//        public Task<Response> SendEmailAsync(string[] emails, string subject, string message, bool sendAsBCC);
//    }
//    public class SendGridEmailSender : IEmailSender
//    {
//        public SendGridEmailSender(IOptions<SendGridEmailSenderOptions> options)
//        {
//            Options = options.Value;
//        }

//        public SendGridEmailSenderOptions Options { get; set; }

//        public async Task<Response> SendEmailAsync(string email, string subject, string message)
//        {
//            return await Execute(Options.ApiKey, subject, message, new string[] { email }, false);
//        }

//        public async Task<Response> SendEmailAsync(string[] emails, string subject, string message, bool sendAsBCC)
//        {
//            return await Execute(Options.ApiKey, subject, message, emails, sendAsBCC);
//        }

//        private async Task<Response> Execute(string apiKey, string subject, string message, string[] emails, bool sendAsBCC)
//        {
//            var client = new SendGridClient(apiKey);
//            var msg = new SendGridMessage()
//            {
//                From = new EmailAddress(Options.SenderEmail, Options.SenderName),
//                Subject = subject,
//                HtmlContent = message,
//            };

//            if (sendAsBCC)
//            {
//                msg.AddTo(new EmailAddress(Options.SenderEmail));
//                msg.AddBccs(emails.Select(x => new EmailAddress(x)).ToList());
//            }
//            else
//            {
//                msg.AddTos(emails.Select(x => new EmailAddress(x)).ToList());
//            }

//            msg.SetClickTracking(false, false);
//            msg.SetOpenTracking(false);
//            msg.SetGoogleAnalytics(false);
//            msg.SetSubscriptionTracking(false);

//            return await client.SendEmailAsync(msg);
//        }
//    }

//    public class SendGridEmailSenderOptions
//    {
//        public string ApiKey { get; set; }
//        public string SenderEmail { get; set; }
//        public string SenderName { get; set; }
//    }
//}
