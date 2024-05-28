using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EcommerceAPI.Core.Auths.Commands
{
    public class RegisterFacebookCommand : IRequest<RequestResult<TokenViewModel>>
    {
        public string AccessToken { get; set; }
        public string NotificationID { get; set; }
        public class RegisterFacebookCommandHandler : IRequestHandler<RegisterFacebookCommand, RequestResult<TokenViewModel>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IConfiguration _configuration;
            public RegisterFacebookCommandHandler(UserManager<AppUser> userManager, IConfiguration configuration)
            {
                _userManager = userManager;
                _configuration = configuration;
            }

            public async Task<RequestResult<TokenViewModel>> Handle(RegisterFacebookCommand command, CancellationToken cancellationToken)
            {
                var r = await HttpHelper.GetFromServer("https://graph.facebook.com/me?fields=photos,email,picture,id,first_name,last_name&access_token=" + command.AccessToken);
                if (r.StatusCode != System.Net.HttpStatusCode.OK)
                    return new RequestResult<TokenViewModel>(ActionResultType.BadRequest, "Invalid Token");

                var u = await HttpHelper.GetDataFromResponse<FacebookUser>(r);
                if (string.IsNullOrEmpty(u?.id))
                    return new RequestResult<TokenViewModel>(ActionResultType.BadRequest, "Invalid User");
                if (string.IsNullOrEmpty(u.email))
                    u.email = u.id + "@facebook.com";

                var user = new AppUser
                {
                    UserName = u.email,
                    Email = u.email,
                    FacebookId = u.id,
                    DisplayName = u.first_name + " " + u.last_name,
                    IsActive = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.Now,
                    UserType = UserTypes.Customer,
                    NotificationID = command.NotificationID,
                };

                var existing = _userManager.Users.FirstOrDefault(x => x.FacebookId == u.id || (!string.IsNullOrEmpty(x.Email) && x.Email == u.email));
                if (existing != null)
                {
                    existing.FacebookId = u.id;
                    existing.NotificationID = command.NotificationID;
                    await _userManager.UpdateAsync(existing);
                    return new RequestResult<TokenViewModel>(await TokenGenerator.CreateToken(existing, _userManager, _configuration));
                }

                var result = await _userManager.CreateAsync(user, TokenGenerator.GeneratePassword(8));
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    return new RequestResult<TokenViewModel>(await TokenGenerator.CreateToken(user, _userManager, _configuration));
                }
                else
                {
                    string error = string.Join('\n', result.Errors.Select(x => x.Code + ": " + x.Description));
                    return new RequestResult<TokenViewModel>(ActionResultType.BadRequest, error);
                }
            }
        }
    }
}
