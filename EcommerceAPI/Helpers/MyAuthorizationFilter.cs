using Hangfire.Dashboard;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace EcommerceAPI.Web.Helpers
{
    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private static readonly string HangFireCookieName = "HangFireCookie";
        private static readonly int CookieExpirationMinutes = 60;
        private TokenValidationParameters tokenValidationParameters;

        public MyAuthorizationFilter(TokenValidationParameters tokenValidationParameters)
        {
            this.tokenValidationParameters = tokenValidationParameters;
        }

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            string access_token;
            var setCookie = false;

            // try to get token from query string
            if (httpContext.Request.Query.ContainsKey("access_token"))
            {
                access_token = httpContext.Request.Query["access_token"].FirstOrDefault();
                setCookie = true;
            }
            else
            {
                access_token = httpContext.Request.Cookies[HangFireCookieName];
            }

            if (string.IsNullOrEmpty(access_token))
                return false;

            try
            {
                var claims = new JwtSecurityTokenHandler().ValidateToken(access_token, this.tokenValidationParameters, out SecurityToken validatedToken);
                if (!claims.Claims.Any(x=>x.Type == "0"))
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            if (setCookie)
            {
                httpContext.Response.Cookies.Append(HangFireCookieName, access_token, new CookieOptions()
                {
                    Expires = DateTime.Now.AddMinutes(CookieExpirationMinutes)
                });
            }
            return true;
        }
    }
}