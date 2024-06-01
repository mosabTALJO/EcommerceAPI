using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EcommerceAPI.Core.Users.Queries
{
    public class GetUserInfoQuery : IRequest<RequestResult<UserVM>>
    {
        public string Id { get; set; }
        public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, RequestResult<UserVM>>
        {
            private readonly UserManager<AppUser> _userManager;
            public GetUserInfoQueryHandler(UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }
            public async Task<RequestResult<UserVM>> Handle(GetUserInfoQuery query, CancellationToken cancellationToken)
            {
                var x = await _userManager.FindByIdAsync(query.Id);
                if (x is null)
                    return new RequestResult<UserVM>(ActionResultType.NotFound);
                var m = new UserVM
                {
                    DisplayName = x.DisplayName,
                    Id = x.Id,
                    IsActive = x.IsActive,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email,
                    UserType = x.UserType,
                    Claims = (await _userManager.GetClaimsAsync(x)).Select(x => (Policies)int.Parse(x.Type)).ToList()
                };
                return new RequestResult<UserVM>(m);
            }
        }
    }
}