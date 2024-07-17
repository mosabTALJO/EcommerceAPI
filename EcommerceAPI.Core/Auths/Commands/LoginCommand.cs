using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Auths.Commands
{
    public class LoginCommand : IRequest<RequestResult<TokenViewModel>>
    {
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public class LoginCommandHandler : IRequestHandler<LoginCommand, RequestResult<TokenViewModel>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IConfiguration _configuration;
            public LoginCommandHandler(UserManager<AppUser> userManager, IConfiguration configuration)
            {
                _userManager = userManager;
                _configuration = configuration;
            }

            public async Task<RequestResult<TokenViewModel>> Handle(LoginCommand command, CancellationToken cancellationToken)
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(x => !string.IsNullOrEmpty(x.Email) && x.Email == command.Email) ??
                            await _userManager.Users.FirstOrDefaultAsync(x => !string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber == command.PhoneNumber);
                if (user is null)
                    return new RequestResult<TokenViewModel>(ActionResultType.NotFound, "Username or password is incorrect");

                if (!await _userManager.CheckPasswordAsync(user, command.Password))
                    return new RequestResult<TokenViewModel>(ActionResultType.Unauthorized, "Username or Password is incorrect");

                if (!user.IsActive)
                    return new RequestResult<TokenViewModel>(ActionResultType.Unauthorized, "User not active");

                var token = await TokenGenerator.CreateToken(user, _userManager, _configuration);
                return new RequestResult<TokenViewModel>(token);
            }
        }
    }
}
