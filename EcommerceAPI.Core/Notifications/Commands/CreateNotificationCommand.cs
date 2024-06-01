using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EcommerceAPI.Core.Notifications.Commands
{
    public class CreateNotificationCommand : IRequest<RequestResult<int>>
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Topic { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }

        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? CollectionId { get; set; }
        public int? ProductId { get; set; }
        public int? OrderId { get; set; }
        public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, RequestResult<int>>
        {
            private readonly IMapper _mapper;
            private readonly UserManager<AppUser> _userManager;
            private readonly INotificationService _notificationService;
            private readonly IUnitOfWork _unitOfWork;

            public CreateNotificationCommandHandler(IMapper mapper, UserManager<AppUser> userManager, INotificationService notificationService, IUnitOfWork unitOfWork)
            {
                _mapper = mapper;
                _userManager = userManager;
                _notificationService = notificationService;
                _unitOfWork = unitOfWork;
            }

            public async Task<RequestResult<int>> Handle(CreateNotificationCommand command, CancellationToken cancellationToken)
            {
                var notification = _mapper.Map<Notification>(command);
                if (!string.IsNullOrWhiteSpace(command.UserId))
                    notification.User = await _userManager.FindByNameAsync(command.UserId);
                else
                    command.Topic = "All";

                await _notificationService.AddNotification(notification);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<int>(notification.Id);
                return new RequestResult<int>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
