using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.Core.Extentions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Users.Queries
{
    public class GetAllUsersQuery : IRequest<RequestResult<ListResponse<UserVM>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Search { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }
        public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, RequestResult<ListResponse<UserVM>>>
        {
            private readonly UserManager<AppUser> _userManager;

            public GetAllUsersQueryHandler(UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<RequestResult<ListResponse<UserVM>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
            {
                var lst = _userManager.Users.Where(x => x.UserType == UserTypes.Admin);
                if (!string.IsNullOrEmpty(query.Search))
                    lst = lst.Where(x => x.DisplayName.Contains(query.Search) || x.Email.Contains(query.Search) || x.PhoneNumber.Contains(query.Search));
                lst = query.SortOrder?.ToLower() == "asc" ? lst.OrderBy(query.SortField) : lst.OrderByDescending(query.SortField);

                lst = lst.Skip(query.PageSize * query.PageNumber).Take(query.PageSize);
                var users = await lst.ToArrayAsync();

                var result = new List<UserVM>();
                foreach (var x in users)
                {
                    var m = new UserVM { DisplayName = x.DisplayName, Id = x.Id, IsActive = x.IsActive, PhoneNumber = x.PhoneNumber, Email = x.Email, UserType = x.UserType, Country = x.Country, CustomerBalance = x.CustomerBalance, };
                    m.Claims = (await _userManager.GetClaimsAsync(x)).Select(x => (Policies)int.Parse(x.Type)).ToList();
                    result.Add(m);
                }

                return new RequestResult<ListResponse<UserVM>>(new ListResponse<UserVM>
                {
                    TotalRecords = lst.Count(),
                    Items = result.ToArray()
                });
            }
        }
    }
}
