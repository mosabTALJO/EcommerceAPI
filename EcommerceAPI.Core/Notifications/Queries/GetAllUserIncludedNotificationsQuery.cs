using AutoMapper;
using AutoMapper.QueryableExtensions;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;

namespace EcommerceAPI.Core.Notifications.Queries
{
    public class GetAllUserIncludedNotificationsQuery : IRequest<RequestResult<ListResponse<NotificationVM>>>
    {
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public string SortField { get; set; } = "Id";
        public string SortOrder { get; set; } = "Asc";
        public string UserId { get; set; }
        public class GetAllUserIncludedNotificationsQueryHandler : IRequestHandler<GetAllUserIncludedNotificationsQuery, RequestResult<ListResponse<NotificationVM>>>
        {
            private readonly IMapper _mapper;
            private readonly INotificationService _notificationService;

            public GetAllUserIncludedNotificationsQueryHandler(IMapper mapper, INotificationService notificationService)
            {
                _mapper = mapper;
                _notificationService = notificationService;
            }

            public async Task<RequestResult<ListResponse<NotificationVM>>> Handle(GetAllUserIncludedNotificationsQuery query, CancellationToken cancellationToken)
            {
                var lst = _notificationService.Queryable().Include(x => x.User).Where(x => x.UserId == query.UserId);

                lst = query.SortOrder?.ToLower() == "asc" ? lst.OrderBy(query.SortField) : lst.OrderByDescending(query.SortField);

                var result = new ListResponse<NotificationVM>
                {
                    TotalRecords = await lst.CountAsync(),
                    Items = await lst.Include(x => x.User).Skip(query.PageSize * query.PageNumber).Take(query.PageSize)
                    .ProjectTo<NotificationVM>(_mapper.ConfigurationProvider).ToArrayAsync()
                };
                return new RequestResult<ListResponse<NotificationVM>>(result);
            }
        }
    }
}
