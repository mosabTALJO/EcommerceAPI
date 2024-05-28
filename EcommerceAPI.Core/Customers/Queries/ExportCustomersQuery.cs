using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Customers.Queries
{
    public class ExportCustomersQuery : IRequest<RequestResult<ExportCustomerVM[]>>
    {
        public int CountryId { get; set; } = 0; //0 = All Countries
        public class ExportCustomersQueryHandler : IRequestHandler<ExportCustomersQuery, RequestResult<ExportCustomerVM[]>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly ICountryService _countryService;
            public ExportCustomersQueryHandler(UserManager<AppUser> userManager, ICountryService countryService)
            {
                _userManager = userManager;
                _countryService = countryService;
            }
            public async Task<RequestResult<ExportCustomerVM[]>> Handle(ExportCustomersQuery query, CancellationToken cancellationToken)
            {
                var c = _countryService.Queryable().FirstOrDefault(x => x.Id == query.CountryId);
                var customers = _userManager.Users.Include(x => x.Orders)
                    .Where(x => x.UserType == UserTypes.Customer && (query.CountryId == 0 || !string.IsNullOrEmpty(x.Country) && (x.Country == c.Code || x.Country == c.NameEn || x.Country == c.NameAr || x.Country == c.CurrencyCode)))
                    .Select(x => new ExportCustomerVM
                    {
                        Id = x.Id,
                        DisplayName = x.DisplayName,
                        Email = x.Email,
                        OrdersCount = x.Orders.Count,
                        Country = x.Country,
                        LastOrderDate = x.Orders.Any() ? x.Orders.OrderByDescending(x => x.CreatedAt).First().CreatedAt : null,
                        PhoneNumber = (x.Country == "AE" ? "00971" :
                                        x.Country == "BH" ? "00973" :
                                        x.Country == "EG" ? "0020" :
                                        x.Country == "EGY" ? "0020" :
                                        x.Country == "KW" ? "00965" :
                                        x.Country == "OM" ? "00968" :
                                        x.Country == "QA" ? "00974" :
                                        x.Country == "SA" ? "00966" :
                                        x.Country == "TR" ? "0090" : "") + x.PhoneNumber,
                    }).ToArray();

                return new RequestResult<ExportCustomerVM[]>(customers);
            }
        }
    }
    public class ExportCustomerVM
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int OrdersCount { get; set; }
        public DateTime? LastOrderDate { get; set; }
    }
}