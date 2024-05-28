using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Auths.Queries
{
    public class RefreshTokenQuery : IRequest<RequestResult<TokenViewModel>>
    {
        public string UserId { get; set; }
        public class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, RequestResult<TokenViewModel>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IConfiguration _configuration;
            public RefreshTokenQueryHandler(UserManager<AppUser> userManager, IConfiguration configuration)
            {
                _userManager = userManager;
                _configuration = configuration;
            }
            public async Task<RequestResult<TokenViewModel>> Handle(RefreshTokenQuery query, CancellationToken cancellationToken)
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == query.UserId);
                if (user is null)
                    return new RequestResult<TokenViewModel>(ActionResultType.NotFound, "User is invalid");
                if (!user.IsActive)
                    return new RequestResult<TokenViewModel>(ActionResultType.Unauthorized, "User is not active");
                var token = await TokenGenerator.CreateToken(user, _userManager, _configuration);
                return new RequestResult<TokenViewModel>(token);
            }
        }
    }
}
