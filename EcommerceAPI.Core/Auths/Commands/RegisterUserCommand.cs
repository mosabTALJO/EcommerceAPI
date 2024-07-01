using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EcommerceAPI.Core.Auths.Commands
{
    public class RegisterUserCommand : IRequest<RequestResult<TokenViewModel>>
    {
        public string DisplayName { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; }
        public string Password { get; set; }
        public string Country { get; set; } = string.Empty;
        public string NotificationID { get; set; } = string.Empty;
        public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RequestResult<TokenViewModel>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IConfiguration _configuration;

            public RegisterUserCommandHandler(UserManager<AppUser> userManager, IConfiguration configuration)
            {
                _userManager = userManager;
                _configuration = configuration;
            }
            public async Task<RequestResult<TokenViewModel>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(command?.DisplayName) || string.IsNullOrEmpty(command?.Email))
                    return new RequestResult<TokenViewModel>(ActionResultType.BadRequest, "Email and/or Name is required");

                var user = new AppUser
                {
                    UserName = command.Email,
                    Email = command.Email,
                    PhoneNumber = command.PhoneNumber,
                    DisplayName = command.DisplayName,
                    IsActive = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.Now,
                    UserType = UserTypes.Customer,
                    NotificationID = command.NotificationID,
                    //Gender = model.Gender,
                    //DOB = model.DOB,
                    Country = command.Country,
                };

                var result = await _userManager.CreateAsync(user, command.Password);
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
