using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EcommerceAPI.Core.Customers.Queries
{
    public class GetCustomerByIdQuery : IRequest<RequestResult<UserVM>>
    {
        public string Id { get; set; }
        public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, RequestResult<UserVM>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;
            public GetCustomerByIdQueryHandler(UserManager<AppUser> userManager, IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
            }
            public async Task<RequestResult<UserVM>> Handle(GetCustomerByIdQuery query, CancellationToken cancellationToken)
            {
                var x = await _userManager.FindByIdAsync(query.Id);
                if (x is null)
                    return new RequestResult<UserVM>(null, ActionResultType.NotFound, "NotFound");
                var m = _mapper.Map<UserVM>(x);
                m.Claims = (await _userManager.GetClaimsAsync(x)).Select(x => (Policies)int.Parse(x.Type)).ToList();
                return new RequestResult<UserVM>(m);
            }
        }
    }
}
