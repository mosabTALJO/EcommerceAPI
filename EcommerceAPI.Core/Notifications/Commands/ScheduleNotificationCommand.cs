using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using MediatR;

namespace EcommerceAPI.Core.Notifications.Commands
{
    public class ScheduleNotificationCommand : IRequest<RequestResult<int>>
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
        public DateTime ScheduledDate { get; set; }
        public class ScheduleNotificationCommandHandler : IRequestHandler<ScheduleNotificationCommand, RequestResult<int>>
        {
            public Task<RequestResult<int>> Handle(ScheduleNotificationCommand request, CancellationToken cancellationToken)
            {
                //var notification = _mapper.Map<Notification>(command);
                //notification.Url = "https://trgatestore.com/products" + (command.ProductId.HasValue ? $"/{command.ProductId}" : $"?categoryId={command.CategoryId}&brandId={command.BrandId}&collectionId={command.CollectionId}");
                //notification.Topic = "all";

                //await _notificationService.AddNotification(notification);
                //if (await _unitOfWork.SaveChangesAsync() > 0)
                //    return new RequestResult<int>(notification.Id);

                //return new RequestResult<int>(ActionResultType.BadRequest, "saving error");
                throw new NotImplementedException();
            }
        }
    }
}
