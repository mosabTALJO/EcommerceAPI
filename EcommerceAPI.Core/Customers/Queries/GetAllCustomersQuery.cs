using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Core.Extentions;

namespace EcommerceAPI.Core.Customers.Queries
{
    public class GetAllCustomersQuery : IRequest<RequestResult<ListResponse<UserVM>>>
    {
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public string Search { get; set; } = "";
        public string SortField { get; set; } = "CreatedAt";
        public string SortOrder { get; set; } = "Asc";
        public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, RequestResult<ListResponse<UserVM>>>
        {
            private readonly UserManager<AppUser> _userManager;
            public GetAllCustomersQueryHandler(UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }
            public async Task<RequestResult<ListResponse<UserVM>>> Handle(GetAllCustomersQuery query, CancellationToken cancellationToken)
            {
                var lst = _userManager.Users.Where(x => x.UserType == UserTypes.Customer);
                if (!string.IsNullOrEmpty(query.Search))
                    lst = lst.Where(x => x.DisplayName.Contains(query.Search) || x.Email.Contains(query.Search) || x.PhoneNumber.Contains(query.Search));
                lst = query.SortOrder?.ToLower() == "asc" ? lst.OrderBy(query.SortField) : lst.OrderByDescending(query.SortField);

                var users = await lst.Skip(query.PageSize * query.PageNumber).Take(query.PageSize).ToArrayAsync();
                var result = new List<UserVM>();
                foreach (var x in users)
                {
                    var m = new UserVM { DisplayName = x.DisplayName, Id = x.Id, IsActive = x.IsActive, PhoneNumber = x.PhoneNumber, Email = x.Email, UserType = x.UserType, Country = x.Country };
                    m.Claims = (await _userManager.GetClaimsAsync(x)).Select(x => (Policies)int.Parse(x.Type)).ToList();

                    m.PhoneNumber = (
                        m.Country == "AE" ? "00971" :
                        m.Country == "BH" ? "00973" :
                        m.Country == "EG" ? "0020" :
                        m.Country == "EGY" ? "0020" :
                        m.Country == "KW" ? "00965" :
                        m.Country == "OM" ? "00968" :
                        m.Country == "QA" ? "00974" :
                        m.Country == "SA" ? "00966" :
                        m.Country == "TR" ? "0090" : "")
                        + m.PhoneNumber;
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