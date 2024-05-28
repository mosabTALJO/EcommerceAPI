using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EcommerceAPI.Core.Auths.Commands
{
    public class ForgetPasswordConfirmCommand : IRequest<RequestResult<bool>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Code { get; set; }
        public class ForgetPasswordConfirmCommandHandler : IRequestHandler<ForgetPasswordConfirmCommand, RequestResult<bool>>
        {
            private readonly UserManager<AppUser> _userManager;

            public ForgetPasswordConfirmCommandHandler(UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<RequestResult<bool>> Handle(ForgetPasswordConfirmCommand command, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(command.Email);
                if (user == null)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "user not found");

                var result = await _userManager.ResetPasswordAsync(user, command.Code, command.Password);
                if (result.Succeeded)
                    return new RequestResult<bool>(true);
                else
                    return new RequestResult<bool>(ActionResultType.BadRequest, result.Errors.ToString());
            }
        }
    }
}
