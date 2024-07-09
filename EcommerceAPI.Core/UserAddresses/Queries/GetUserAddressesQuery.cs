using AutoMapper;
using EcommerceAPI.Core.Extentions;
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
            private readonly IMapper mapper;

            public GetUserAddressesQueryHandler(IUserAddressService userAddressService, IMapper mapper)
            {
                _userAddressService = userAddressService;
                this.mapper = mapper;
            }

            public async Task<RequestResult<UserAddressVM[]>> Handle(GetUserAddressesQuery query, CancellationToken cancellationToken)
            {
                if (query.UserId is null) return new RequestResult<UserAddressVM[]>(ActionResultType.BadRequest);

                var lst = _userAddressService.Queryable().Where(x => x.AppUserId == query.UserId);
                var lstResult = lst.To<UserAddressVM>(mapper);
                return new RequestResult<UserAddressVM[]>(await lstResult.ToArrayAsync());
            }
        }
    }
}
