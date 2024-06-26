using System.Net;
using System.Text.Json;

namespace EcommerceAPI.Web.Helpers
{
    public class ErrorHandlerMiddleware
    {
        private readonly ILogger<ErrorHandlerMiddleware> _log;
        private readonly RequestDelegate _next;
        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> log)
        {
            _next = next;
            _log = log;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                using (var timoutTS = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted))
                {
                    timoutTS.CancelAfter(TimeSpan.FromMinutes(2));
                    context.RequestAborted = timoutTS.Token;
                    await _next(context);
                }
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.BadRequest;

                var result = error?.Message + ". " + error?.InnerException?.Message + ". " + error?.StackTrace;

                _log.LogError($"Mothod: {context?.Request?.Path}, Error: {error?.Message}, Inner Exception: {error?.InnerException?.Message}");
                _log.LogError("====================================================================== TRACE");
                _log.LogError(error?.StackTrace);
                _log.LogError("====================================================================== END");

                await response.WriteAsync(JsonSerializer.Serialize(result));
            }
        }
    }
}