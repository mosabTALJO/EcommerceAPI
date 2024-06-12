using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceAPI.Shared.Helpers.Abstractions
{
    public interface IAppEmailSender
    {
        public Task<EmailResponse> SendEmailAsync(string email, string subject, string message);
        public Task<EmailResponse> SendEmailAsync(string[] emails, string subject, string message, bool sendAsBCC);
    }
    public class EmailResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Body { get; set; }
    }
}