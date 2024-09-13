using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace EcommerceAPI.Core.Auths.Commands
{
    public class RegisterGoogleCommand : IRequest<RequestResult<TokenViewModel>>
    {
        public string IdToken { get; set; }
        public class RegisterGoogleCommandHandler : IRequestHandler<RegisterGoogleCommand, RequestResult<TokenViewModel>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IConfiguration _configuration;

            public RegisterGoogleCommandHandler(UserManager<AppUser> userManager, IConfiguration configuration)
            {
                _userManager = userManager;
                _configuration = configuration;
            }
            public async Task<RequestResult<TokenViewModel>> Handle(RegisterGoogleCommand command, CancellationToken cancellationToken)
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(command.IdToken);
                var issuer = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "iss")?.Value;
                var sub = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;
                var email = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
                var displayName = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "name")?.Value;

                if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(sub))
                    return new RequestResult<TokenViewModel>(ActionResultType.BadRequest, "Invalid User");
                if (string.IsNullOrEmpty(email))
                    email = sub + "@google.com";
                if (string.IsNullOrEmpty(displayName))
                    displayName = email;

                var user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    GoogleId = sub,
                    DisplayName = displayName,
                    IsActive = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow,
                    UserType = UserTypes.Customer,
                };

                var existing = _userManager.Users.FirstOrDefault(x => (!string.IsNullOrEmpty(sub) && x.GoogleId == sub) || (!string.IsNullOrEmpty(x.Email) && !string.IsNullOrEmpty(email) && x.Email == email));
                if (existing != null)
                {
                    existing.GoogleId = sub;
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
