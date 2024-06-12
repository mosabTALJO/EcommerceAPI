using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.UserAddresses.Queries
{
    public class GetUserAddressesQuery : IRequest<RequestResult<UserAddressVM[]>>
    {
        public string UserId { get; set; }
        public class GetUserAddressesQueryHandler : IRequestHandler<GetUserAddressesQuery, RequestResult<UserAddressVM[]>>
        {
            private readonly IUserAddressService _userAddressService;

            public GetUserAddressesQueryHandler(IUserAddressService userAddressService)
            {
                _userAddressService = userAddressService;
            }

            public async Task<RequestResult<UserAddressVM[]>> Handle(GetUserAddressesQuery query, CancellationToken cancellationToken)
            {
                if (query.UserId is null) return new RequestResult<UserAddressVM[]>(ActionResultType.BadRequest);

                var lst = _userAddressService.Queryable().Where(x => x.AppUserId == query.UserId);
                var lstResult = lst.Select(x => new UserAddressVM
                {
                    Id = x.Id,
                    Name = x.FirstName + "" + x.LastName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Country = x.Country,
                    City = x.City,
                    FullAddress = x.FullAddress,
                    PhoneNumber = x.PhoneNumber,
                    IsDefault = x.IsDefault,
                    Lat = x.Lat,
                    Lng = x.Lng
                });
                return new RequestResult<UserAddressVM[]>(await lstResult.ToArrayAsync());
            }
        }
    }
}
