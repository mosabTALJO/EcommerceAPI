using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EcommerceAPI.Core.Users.Command
{
    public class UpdateUserCommand : IRequest<RequestResult<bool>>
    {
        public void SetUserId(string UserId) =>this.UserId = UserId;
        public string? DisplayName { get; set; }
        public string? PhoneNumber { get; set; }
        public required string Email { get; set; }
        public string? Password { get; set; }
        public required string Country { get; set; }
        private string UserId { get; set; } = string.Empty;
        public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, RequestResult<bool>>
        {
            private readonly UserManager<AppUser> _userManager;

            public UpdateUserCommandHandler(UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<RequestResult<bool>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
            {
                var u = await _userManager.FindByIdAsync(command.UserId);
                if (u is null)
                    return new RequestResult<bool>(ActionResultType.NotFound);
                if (u.UserName == "admin")
                    return new RequestResult<bool>(ActionResultType.NotFound, "AdminUserUpdateError");
                if (!u.IsActive)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "InActiveUser");
                if (string.IsNullOrWhiteSpace(command.DisplayName) || string.IsNullOrWhiteSpace(command.Email))
                    return new RequestResult<bool>(ActionResultType.BadRequest, "DataValidationError");

                u.DisplayName = command.DisplayName;
                u.Email = command.Email;
                u.UserName = command.Email;
                u.PhoneNumber = command.PhoneNumber;
                u.Country = command.Country;

                if (!string.IsNullOrEmpty(command.Password) && !await _userManager.CheckPasswordAsync(u, command.Password))
                {
                    var xx = await _userManager.ResetPasswordAsync(u, await _userManager.GeneratePasswordResetTokenAsync(u), command.Password);
                    if (!xx.Succeeded)
                        return new RequestResult<bool>(ActionResultType.BadRequest, xx.Errors.FirstOrDefault()?.Description ?? "Error");
                }

                var r = await _userManager.UpdateAsync(u);
                if (r.Succeeded)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, string.Join(", ", r.Errors.Select(x => x.Description)));
            }
        }
    }
}
