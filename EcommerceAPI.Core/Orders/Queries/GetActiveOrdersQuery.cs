using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.Services;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Core.Extentions;

namespace EcommerceAPI.Core.Orders.Queries
{
    public class GetActiveOrdersQuery : IRequest<List<OrderVM>>
    {
        public string Country { get; set; } = null;
        public string City { get; set; } = null;
        public string Search { get; set; } = null;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public class GetActiveOrdersQueryHandler : IRequestHandler<GetActiveOrdersQuery, List<OrderVM>>
        {
            private readonly IOrderService _orderService;
            private readonly IMapper _mapper;
            public GetActiveOrdersQueryHandler(IOrderService orderService, IMapper mapper)
            {
                _orderService = orderService;
                _mapper = mapper;
            }
            public async Task<List<OrderVM>> Handle(GetActiveOrdersQuery query, CancellationToken cancellationToken)
            {
                var lst = _orderService.Queryable().AsNoTracking()
                    .Include(x => x.Customer)
                    .Include(o => o.OrderItems).ThenInclude(x => x.Product)
                    .Where(x => !x.DeletedAt.HasValue)
                    .Where(o => !o.IsArchived)
                    .Where(x => query.StartDate == null || x.CreatedAt >= query.StartDate.Value.Date)
                    .Where(x => query.EndDate == null || x.CreatedAt < query.EndDate.Value.Date.AddDays(1))
                    .Where(o => o.OrderItems.Any(oi => !oi.ShipmentId.HasValue && oi.OrderItemState == OrderItemState.InStock))
                    .Where(o => o.OrderItems.Count > 0);

                if (!string.IsNullOrEmpty(query.Country))
                    lst = lst.Where(x => x.Country == query.Country);
                if (!string.IsNullOrEmpty(query.City))
                    lst = lst.Where(x => x.City == query.City);
                if (!string.IsNullOrEmpty(query.Search))
                    lst = lst.Where(x => x.Id.ToString().Contains(query.Search));

                var result = await lst.To<OrderVM>(_mapper).ToListAsync();
                foreach (var o in result)
                {
                    o.OrderItems = o.OrderItems.Where(x => !x.ShipmentId.HasValue && x.OrderItemState == OrderItemState.InStock).ToList();
                    foreach (var oi in o.OrderItems)
                        oi.Product.MainImage = string.IsNullOrWhiteSpace(oi.Product.MainImage) || oi.Product.MainImage.StartsWith("https") ? oi.Product.MainImage : $"https://cdn.dsmcdn.com/mnresize/{600}/{500}" + oi.Product.MainImage;
                }
                return result;
            }
        }
    }
}