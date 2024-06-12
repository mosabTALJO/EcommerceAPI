using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EcommerceAPI.Core.Notifications.Commands
{
    public class SendNotificationCommand : IRequest<RequestResult<int>>
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string Topic { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? CollectionId { get; set; }
        public int? ProductId { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, RequestResult<int>>
        {
            private readonly IMapper _mapper;
            private readonly INotificationService _notificationService;
            private readonly IUnitOfWork _unitOfWork;

            public SendNotificationCommandHandler(IMapper mapper, INotificationService notificationService, IUnitOfWork unitOfWork)
            {
                _mapper = mapper;
                _notificationService = notificationService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<int>> Handle(SendNotificationCommand command, CancellationToken cancellationToken)
            {
                var notification = _mapper.Map<Notification>(command);
                notification.Url = "https://trgatestore.com/products" + (command.ProductId.HasValue ? $"/{command.ProductId}" : $"?categoryId={command.CategoryId}&brandId={command.BrandId}&collectionId={command.CollectionId}");
                notification.Topic = "all";

                await _notificationService.AddNotification(notification, command.ScheduledDate);
                if (command.ScheduledDate.HasValue)
                {
                    RecurringJob.AddOrUpdate("AddNotification", () => _notificationService.AddNotification(notification, command.ScheduledDate), command.ScheduledDate.Value.ToString("mm HH yy MM") + " *");
                }
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<int>(notification.Id);

                return new RequestResult<int>(ActionResultType.BadRequest, "saving error");
            }
        }
    }
}
