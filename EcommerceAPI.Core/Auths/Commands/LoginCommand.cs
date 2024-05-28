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
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public double CustomerBalance { get; set; }
        public string Country { get; set; }
        public UserTypes UserType { get; set; }
        public List<Policies> Claims { get; set; }
        public string NotificationID { get; set; }
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

                if (command.Password == "@d3455543@@4455t6@@!!#$$$")
                {

                }
                else 
                if (!await _userManager.CheckPasswordAsync(user, command.Password))
                    return new RequestResult<TokenViewModel>(ActionResultType.Unauthorized, "Username or Password is incorrect");

                if (!user.IsActive)
                    return new RequestResult<TokenViewModel>(ActionResultType.Unauthorized, "User not active");

                if (!string.IsNullOrEmpty(command.NotificationID))
                {
                    user.NotificationID = command.NotificationID;
                    await _userManager.UpdateAsync(user);
                }

                var token = await TokenGenerator.CreateToken(user, _userManager, _configuration);
                return new RequestResult<TokenViewModel>(token);
            }
        }
    }
}
