using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EcommerceAPI.Core.Users.Queries
{
    public class GetMyUserInfoQuery : IRequest<RequestResult<UserVM>>
    {
        public string UserId { get; set; }
        public class GetMyUserInfoQueryHandler : IRequestHandler<GetMyUserInfoQuery, RequestResult<UserVM>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;
            public GetMyUserInfoQueryHandler(UserManager<AppUser> userManager, IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
            }
            public async Task<RequestResult<UserVM>> Handle(GetMyUserInfoQuery query, CancellationToken cancellationToken)
            {
                var u = await _userManager.FindByIdAsync(query.UserId);
                if (u is null)
                    return new RequestResult<UserVM>(ActionResultType.NotFound);
                if (!u.IsActive)
                    return new RequestResult<UserVM>(ActionResultType.BadRequest, "InActiveUser");
                return new RequestResult<UserVM>(_mapper.Map<UserVM>(u));
            }
        }
    }
}