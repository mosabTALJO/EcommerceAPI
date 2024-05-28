using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EcommerceAPI.Core.Carts.Queries
{
    public class GetOrderDetailsNoAccountQuery : IRequest<RequestResult<OrderVM>>
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public class GetOrderStatusNoAccountQueryHadnler : IRequestHandler<GetOrderDetailsNoAccountQuery, RequestResult<OrderVM>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IOrderService _orderService;
            private readonly IMapper _mapper;
            public GetOrderStatusNoAccountQueryHadnler(UserManager<AppUser> userManager, IOrderService orderService, IMapper mapper)
            {
                _userManager = userManager;
                _orderService = orderService;
                _mapper = mapper;
            }

            public async Task<RequestResult<OrderVM>> Handle(GetOrderDetailsNoAccountQuery query, CancellationToken cancellationToken)
            {
                var u = await _userManager.FindByIdAsync(query.UserId);
                if (u == null)
                    return new RequestResult<OrderVM>(ActionResultType.NotFound, "user not found");

                var o = await _orderService.GetOrderDetails(query.OrderId);
                if (o == null)
                    return new RequestResult<OrderVM>(ActionResultType.NotFound, "order not found");

                if (o.CustomerId != u.Id)
                    return new RequestResult<OrderVM>(ActionResultType.BadRequest, "Order Not Found");

                var oVM = _mapper.Map<OrderVM>(o);
                foreach (var oi in oVM.OrderItems)
                {
                    oi.Currency = o.Currency;
                    oi.Product.CompanyURL = string.IsNullOrWhiteSpace(oi.Product.CompanyURL) || oi.Product.CompanyURL.StartsWith("https") ? oi.Product.CompanyURL : "https://www.trendyol.com" + (oi.Product.CompanyURL?.StartsWith("/") == true ? "" : "/") + oi.Product.CompanyURL;
                }

                return new RequestResult<OrderVM>(oVM);
            }
        }
    }
}
