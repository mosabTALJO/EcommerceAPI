using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks.Dataflow;

namespace EcommerceAPI.Core.Customers.Commands
{
    public class UpdateCustomerCommand : IRequest<RequestResult<bool>>
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, RequestResult<bool>>
        {
            private readonly UserManager<AppUser> _userManager;
            public UpdateCustomerCommandHandler(UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }
            public async Task<RequestResult<bool>> Handle(UpdateCustomerCommand command, CancellationToken cancellationToken)
            {
                var u = await _userManager.FindByIdAsync(command.Id);
                if (u is null)
                    return new RequestResult<bool>(false, ActionResultType.NotFound, "NotFound");
                if (u.UserName == "admin")
                    return new RequestResult<bool>(false, ActionResultType.BadRequest, "AdminUserUpdateError");

                u.DisplayName = command.DisplayName;
                u.PhoneNumber = command.PhoneNumber;
                u.Email = command.Email;
                u.UserName = command.Email;

                if (!string.IsNullOrEmpty(command.Password) && !await _userManager.CheckPasswordAsync(u, command.Password))
                {
                    var xx = await _userManager.ResetPasswordAsync(u, await _userManager.GeneratePasswordResetTokenAsync(u), command.Password);
                    if (!xx.Succeeded)
                        return new RequestResult<bool>(false, ActionResultType.BadRequest, xx.Errors.FirstOrDefault()?.Description ?? "Error");
                }

                var r = await _userManager.UpdateAsync(u);
                if (r.Succeeded)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(false, ActionResultType.BadRequest, "EditError");
            }
        }
    }
}
