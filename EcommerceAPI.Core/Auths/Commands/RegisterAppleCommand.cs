using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace EcommerceAPI.Core.Auths.Commands
{
    public class RegisterAppleCommand : IRequest<RequestResult<TokenViewModel>>
    {
        public string IdToken { get; set; }
        public class RegisterAppleCommandHandler : IRequestHandler<RegisterAppleCommand, RequestResult<TokenViewModel>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IConfiguration _configuration;

            public RegisterAppleCommandHandler(UserManager<AppUser> userManager, IConfiguration configuration)
            {
                _userManager = userManager;
                _configuration = configuration;
            }
            public async Task<RequestResult<TokenViewModel>> Handle(RegisterAppleCommand command, CancellationToken cancellationToken)
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(command.IdToken);

                var iss = jwtSecurityToken?.Claims?.FirstOrDefault(x => x.Type == "iss")?.Value;
                var email = jwtSecurityToken?.Claims?.FirstOrDefault(x => x.Type == "email")?.Value;
                var sub = jwtSecurityToken?.Claims?.FirstOrDefault(x => x.Type == "sub")?.Value;
                string displayName = email?.Contains('@') == true ? email?.Remove(email.IndexOf('@')) : "Apple User";

                if (string.IsNullOrEmpty(iss) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sub) || iss != "https://appleid.apple.com")
                    return new RequestResult<TokenViewModel>(ActionResultType.BadRequest, "Invalid Token");

                if (string.IsNullOrEmpty(email))
                    email = sub + "@apple.com";

                var user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    AppleId = sub,
                    DisplayName = displayName,
                    IsActive = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow,
                    UserType = UserTypes.Customer,
                };

                var existing = _userManager.Users.FirstOrDefault(x => x.AppleId == sub || (!string.IsNullOrEmpty(x.Email) && x.Email == email));
                if (existing != null)
                {
                    existing.AppleId = sub;
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
