using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace EcommerceAPI.Core.Users.Command
{
    public class CreateAdminUserCommand : IRequest<RequestResult<string>>
    {
        public string DisplayName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; }
        public string Email { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public double CustomerBalance { get; set; }
        public string Country { get; set; } = string.Empty;
        public UserTypes UserType { get; set; }
        public List<Policies> Claims { get; set; }
        public string UserId { get; set; } = string.Empty;
        public class CreateAdminUserCommandHandler : IRequestHandler<CreateAdminUserCommand, RequestResult<string>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMemoryCache _memoryCache;
            public CreateAdminUserCommandHandler(UserManager<AppUser> userManager, IMemoryCache memoryCache)
            {
                _userManager = userManager;
                _memoryCache = memoryCache;
            }
            public async Task<RequestResult<string>> Handle(CreateAdminUserCommand command, CancellationToken cancellationToken)
            {
                var user = new AppUser
                {
                    UserName = command.Email,
                    Email = command.Email,
                    PhoneNumber = command.PhoneNumber,
                    DisplayName = command.DisplayName,
                    IsActive = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedUser = command.UserId ?? "Admin",
                    CreatedAt = DateTime.UtcNow,
                    UserType = UserTypes.Admin,
                };

                var result = await _userManager.CreateAsync(user, command.Password);
                if (result.Succeeded)
                {
                    if (command.Claims?.Any() == true)
                    {
                        var claims = new List<Claim>();
                        command.Claims?.ForEach(x => claims.Add(new Claim(x.GetPolicyNumber(), x.GetPolicyNumber())));
                        await _userManager.AddClaimsAsync(user, claims);
                    }

                    _memoryCache.Set(user.Id, command.Claims.Select(e => e.GetPolicyNumber()).ToArray());
                    return new RequestResult<string>(user.Id);
                }
                else
                {
                    string error = string.Join('\n', result.Errors.Select(x => x.Code + ": " + x.Description));
                    return new RequestResult<string>(ActionResultType.BadRequest, error);
                }
            }
        }
    }
}
