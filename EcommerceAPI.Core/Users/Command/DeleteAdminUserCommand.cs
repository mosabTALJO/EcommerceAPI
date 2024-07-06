using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace EcommerceAPI.Core.Users.Command
{
    public class DeleteAdminUserCommand : IRequest<RequestResult<bool>>
    {
        public string Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public class DeleteAdminUserCommandHandler : IRequestHandler<DeleteAdminUserCommand, RequestResult<bool>>
        {
            private readonly UserManager<AppUser> _userManager;
            public readonly IMemoryCache _memoryCache;
            public DeleteAdminUserCommandHandler(UserManager<AppUser> userManager, IMemoryCache memoryCache)
            {
                _userManager = userManager;
                _memoryCache = memoryCache;
            }
            public async Task<RequestResult<bool>> Handle(DeleteAdminUserCommand command, CancellationToken cancellationToken)
            {
                var currentUser = await _userManager.FindByIdAsync(command.UserId);
                if (currentUser is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "currentUser NotFound");

                var currentUserClaims = await _userManager.GetClaimsAsync(currentUser);
                if (currentUserClaims == null)
                    return new RequestResult<bool>(ActionResultType.BadRequest, $"oldClaims null");
                if (currentUserClaims.Count == 0)
                    return new RequestResult<bool>(ActionResultType.BadRequest, $"oldClaims empty");
                if (!currentUserClaims.Any(x => x.Type == PoliciesText.EditAdmins))
                    return new RequestResult<bool>(ActionResultType.BadRequest, $"you are not authorized to make changes - {string.Join(',', currentUserClaims.Select(x => x.Type))}");

                var u = await _userManager.FindByIdAsync(command.Id);
                if (u is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "NoAdminUserWithId : " + command.Id);

                var oldClaims = await _userManager.GetClaimsAsync(u);

                var r = await _userManager.DeleteAsync(u);
                if (r.Succeeded)
                {
                    _memoryCache.Remove(command.Id);
                    return new RequestResult<bool>(true);
                }
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
