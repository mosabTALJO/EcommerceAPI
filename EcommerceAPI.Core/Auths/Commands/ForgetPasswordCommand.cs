using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.Helpers.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace EcommerceAPI.Core.Auths.Commands
{
    public class ForgetPasswordCommand : IRequest<RequestResult<bool>>
    {
        public string Email { get; set; }
        public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, RequestResult<bool>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IEmailSender _emailSender;

            public ForgetPasswordCommandHandler(UserManager<AppUser> userManager, IEmailSender emailSender)
            {
                _userManager = userManager;
                _emailSender = emailSender;
            }
            public async Task<RequestResult<bool>> Handle(ForgetPasswordCommand command, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(command.Email);
                if (user == null)//|| !(await _userManager.IsEmailConfirmedAsync(user)))
                    return new RequestResult<bool>(ActionResultType.BadRequest, "user not found");
                if (user.UserType != UserTypes.Customer)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "An error occured when trying to reset the password to this user");

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = $"https://cp.trgatestore.com/auth/forgot-password?code={HttpUtility.UrlEncode(code)}&email={command.Email}";
                string msg = @$"Dear <b>{user.DisplayName}</b>,<br/><br/>
                            Please click the following link to reset your password
                            <a href='{callbackUrl}'>reset your password!</a>
                            <br/><br/><br/>Thank you
                            <br/>TRGate app";
                await _emailSender.SendEmailAsync(command.Email, "Forget password request", msg);
                return new RequestResult<bool>(true);
            }
        }
    }
}
