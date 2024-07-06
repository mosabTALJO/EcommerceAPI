using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace EcommerceAPI.Core.Customers.Commands
{
    public class DeleteCustomerCommand : IRequest<RequestResult<bool>>
    {
        public string Id { get; set; }
        public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, RequestResult<bool>>
        {
            private readonly UserManager<AppUser> _userManager;
            public readonly IMemoryCache _memoryCache;
            public DeleteCustomerCommandHandler(UserManager<AppUser> userManager, IMemoryCache memoryCache)
            {
                _userManager = userManager;
                _memoryCache = memoryCache;
            }
            public async Task<RequestResult<bool>> Handle(DeleteCustomerCommand command, CancellationToken cancellationToken)
            {
                var u = await _userManager.FindByIdAsync(command.Id);
                if (u is null)
                    return new RequestResult<bool>(false, ActionResultType.NotFound, "The user does not exist");
                if (!u.IsActive)
                    return new RequestResult<bool>(false, ActionResultType.BadRequest, "The user is not active");

                if (u.Orders?.Any() != true)
                {
                    var deleteResult = await _userManager.DeleteAsync(u);
                    if (deleteResult.Succeeded)
                        return new RequestResult<bool>(true);
                }

                var t = "deletedOn" + DateTime.UtcNow.ToString("yyyMMddhhmmss");
                u.Email = u.Email + t;
                u.UserName = u.Email;
                u.AppleId = u.AppleId + t;
                u.FacebookId = u.FacebookId + t;
                u.GoogleId = u.GoogleId + t;
                u.IsActive = false;

                var r = await _userManager.UpdateAsync(u);
                if (r.Succeeded)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(false, ActionResultType.BadRequest, "Error");
            }
        }
    }
}