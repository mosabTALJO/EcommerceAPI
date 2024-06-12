using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.Services;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Shared.Helpers;

namespace EcommerceAPI.Core.Orders.Queries
{
    public class GetActiveOrdersQuery : IRequest<RequestResult<List<OrderVM>>>
    {
        public string Country { get; set; } = null;
        public string City { get; set; } = null;
        public string Search { get; set; } = null;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public class GetActiveOrdersQueryHandler : IRequestHandler<GetActiveOrdersQuery, RequestResult<List<OrderVM>>>
        {
            private readonly IOrderService _orderService;
            private readonly IMapper _mapper;
            public GetActiveOrdersQueryHandler(IOrderService orderService, IMapper mapper)
            {
                _orderService = orderService;
                _mapper = mapper;
            }
            public async Task<RequestResult<List<OrderVM>>> Handle(GetActiveOrdersQuery query, CancellationToken cancellationToken)
            {
                var temp = _orderService.ItemsQueryable()
                    .Where(x => !x.Order.DeletedAt.HasValue)
                    .Where(x => !x.Order.IsArchived)
                    .Where(x => query.StartDate == null || x.Order.CreatedAt >= query.StartDate.Value.Date)
                    .Where(x => query.EndDate == null || x.Order.CreatedAt < query.EndDate.Value.Date.AddDays(1))
                    .Where(x => x.OrderItemState == OrderItemState.InStock);

                if (!string.IsNullOrEmpty(query.Country))
                    temp = temp.Where(x => x.Order.Country == query.Country);
                if (!string.IsNullOrEmpty(query.City))
                    temp = temp.Where(x => x.Order.City == query.City);
                if (!string.IsNullOrEmpty(query.Search))
                    temp = temp.Where(x => x.OrderId.ToString().Contains(query.Search));

                var orderIds = await temp.Select(x => x.OrderId).ToArrayAsync();
                var result = await _orderService.Queryable().AsNoTracking()
                    .Include(x => x.Customer)
                    .Include(o => o.OrderItems).ThenInclude(x => x.ProductVariant)
                    .Where(x => EF.Constant(orderIds).Contains(x.Id))
                    .To<OrderVM>(_mapper).ToListAsync();

                foreach (var o in result)
                {
                    o.OrderItems = o.OrderItems.Where(x => !x.ShipmentId.HasValue && x.OrderItemState == OrderItemState.InStock).ToList();
                    foreach (var oi in o.OrderItems)
                        oi.Product.MainImage = string.IsNullOrWhiteSpace(oi.Product.MainImage) || oi.Product.MainImage.StartsWith("https") ? oi.Product.MainImage : $"https://cdn.dsmcdn.com/mnresize/{600}/{500}" + oi.Product.MainImage;
                }
                return new RequestResult<List<OrderVM>>(result);
            }
        }
    }
}