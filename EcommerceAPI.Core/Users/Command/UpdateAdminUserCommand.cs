using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace EcommerceAPI.Core.Users.Command
{
    public class UpdateAdminUserCommand : IRequest<RequestResult<bool>>
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
        public string UserId { get; set; }
        public class UpdateAdminUserCommandHandler : IRequestHandler<UpdateAdminUserCommand, RequestResult<bool>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMemoryCache _memoryCache;
            public UpdateAdminUserCommandHandler(UserManager<AppUser> userManager, IMemoryCache memoryCache)
            {
                _userManager = userManager;
                _memoryCache = memoryCache;
            }
            public async Task<RequestResult<bool>> Handle(UpdateAdminUserCommand command, CancellationToken cancellationToken)
            {
                var u = await _userManager.FindByIdAsync(command.Id);
                if (u is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "NotFound");

                var oldClaims = await _userManager.GetClaimsAsync(u);
                if (oldClaims?.Any(x => x.Value == PoliciesText.EditAdmins) != true)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "you are not authorized to make changes");

                if (u.UserName != "admin")
                {
                    u.IsActive = command.IsActive;
                }

                u.DisplayName = command.DisplayName;
                u.PhoneNumber = command.PhoneNumber;
                u.Email = command.Email;
                u.UserName = command.Email;


                if (!string.IsNullOrEmpty(command.Password) && !await _userManager.CheckPasswordAsync(u, command.Password))
                {
                    var xx = await _userManager.ResetPasswordAsync(u, await _userManager.GeneratePasswordResetTokenAsync(u), command.Password);
                    if (!xx.Succeeded)
                        return new RequestResult<bool>(ActionResultType.BadRequest, xx.Errors.FirstOrDefault()?.Description ?? "Error");
                }

                var r = await _userManager.UpdateAsync(u);
                if (r.Succeeded)
                {
                    if (oldClaims?.Any(x => x.Value == PoliciesText.AdminPermissions) != true)
                        return new RequestResult<bool>(true);
                    if (oldClaims?.Any() == true)
                        await _userManager.RemoveClaimsAsync(u, oldClaims);

                    var claims = new List<Claim>();
                    command.Claims?.ForEach(x => claims.Add(new Claim(x.GetPolicyNumber(), x.GetPolicyNumber())));
                    if (command.UserId == command.Id)
                    {
                        if (command.Claims?.Any(x => x == Policies.AdminPermissions) != true)
                            claims.Add(new Claim(Policies.AdminPermissions.GetPolicyNumber(), Policies.AdminPermissions.GetPolicyNumber()));
                        if (command.Claims?.Any(x => x == Policies.EditAdmins) != true)
                            claims.Add(new Claim(Policies.EditAdmins.GetPolicyNumber(), Policies.EditAdmins.GetPolicyNumber()));
                    }
                    await _userManager.AddClaimsAsync(u, claims);

                    _memoryCache.Remove(command.Id);
                    _memoryCache.Set(command.Id, command.Claims.Select(e => e.GetPolicyNumber()).ToArray());
                    return new RequestResult<bool>(true);
                }
                return
                    new RequestResult<bool>(ActionResultType.BadRequest, "EditError");
            }
        }
    }
}
