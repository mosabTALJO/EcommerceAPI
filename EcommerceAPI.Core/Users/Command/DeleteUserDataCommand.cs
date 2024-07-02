using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EcommerceAPI.Core.Users.Command
{
    public class DeleteUserDataCommand : IRequest<RequestResult<bool>>
    {
        public string Id { get; set; }
        public class DeleteUserDataCommandHandler : IRequestHandler<DeleteUserDataCommand, RequestResult<bool>>
        {
            private readonly UserManager<AppUser> _userManager;
            public DeleteUserDataCommandHandler(UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }
            public async Task<RequestResult<bool>> Handle(DeleteUserDataCommand command, CancellationToken cancellationToken)
            {
                var u = _userManager.Users.FirstOrDefault(x => x.Id == command.Id);
                if (u is null)
                    throw new Exception("The user does not exist");
                if (!u.IsActive)
                    throw new Exception("The user is not active");

                var t = "deletedOn" + DateTime.UtcNow.ToString("yyyMMddhhmmss");
                u.Email += t;
                u.UserName = u.Email;
                u.AppleId += t;
                u.FacebookId += t;
                u.GoogleId += t;
                u.IsActive = false;

                var xx = await _userManager.UpdateAsync(u);
                if (xx.Succeeded)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, xx.Errors.FirstOrDefault()?.Description) { };
            }
        }
    }
}