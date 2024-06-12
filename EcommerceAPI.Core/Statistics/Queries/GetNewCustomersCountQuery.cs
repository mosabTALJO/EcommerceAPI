using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Statistics.Queries
{
    public class GetNewCustomersCountQuery : IRequest<RequestResult<DayWeekMonth>>
    {
        public class GetNewCustomersCountQueryHandler : IRequestHandler<GetNewCustomersCountQuery, RequestResult<DayWeekMonth>>
        {
            public readonly UserManager<AppUser> _userManager;
            public GetNewCustomersCountQueryHandler(UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<RequestResult<DayWeekMonth>> Handle(GetNewCustomersCountQuery request, CancellationToken cancellationToken)
            {
                return new RequestResult<DayWeekMonth>(new DayWeekMonth
                {
                    Day = await _userManager.Users.Where(x => x.CreatedAt >= DateTime.Now.AddDays(-1)).CountAsync(),
                    Week = await _userManager.Users.Where(x => x.CreatedAt >= DateTime.Now.AddDays(-7)).CountAsync(),
                    Month = await _userManager.Users.Where(x => x.CreatedAt >= DateTime.Now.AddDays(-30)).CountAsync(),
                });
            }
        }
    }
    public class DayWeekMonth
    {
        public int Day { get; set; }
        public int Week { get; set; }
        public int Month { get; set; }
    }
}
