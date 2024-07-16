using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Auths.Commands
{
    public class LoginAdminCommand : IRequest<RequestResult<TokenViewModel>>
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public class LoginAdminCommandHandler : IRequestHandler<LoginAdminCommand, RequestResult<TokenViewModel>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IConfiguration _configuration;
            public LoginAdminCommandHandler(UserManager<AppUser> userManager, IConfiguration configuration)
            {
                _userManager = userManager;
                _configuration = configuration;
            }
            public async Task<RequestResult<TokenViewModel>> Handle(LoginAdminCommand command, CancellationToken cancellationToken)
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(x => !string.IsNullOrEmpty(x.Email) && x.Email == command.Email && x.UserType == UserTypes.Admin) ??
                            await _userManager.Users.FirstOrDefaultAsync(x => !string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber == command.PhoneNumber && x.UserType == UserTypes.Admin);
                if (user is null)
                    return new RequestResult<TokenViewModel>(ActionResultType.NotFound, "Username not found!");

                if (!await _userManager.CheckPasswordAsync(user, command.Password))
                    return new RequestResult<TokenViewModel>(ActionResultType.Unauthorized, "Username or Password is incorrect!");

                if (!user.IsActive)
                    return new RequestResult<TokenViewModel>(ActionResultType.Unauthorized, "User not active");

                if(user.UserType != UserTypes.Admin)
                    return new RequestResult<TokenViewModel>(ActionResultType.Unauthorized, "User not admin!");

                var token = await TokenGenerator.CreateToken(user, _userManager, _configuration);
                return new RequestResult<TokenViewModel>(token);
            }
        }
    }
}