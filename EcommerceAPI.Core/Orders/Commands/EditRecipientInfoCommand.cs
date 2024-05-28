using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class EditRecipientInfoCommand : IRequest<RequestResult<bool>>
    {
        public int OrderId { get; set; }
        public string NewPhoneNumber { get; set; }
        public string NewFirstName { get; set; }
        public string NewLastName { get; set; }
        public string NewaAdderss { get; set; }
        public string UserId { get; set; }
        public class EditRecipientInfoCommandHandler : IRequestHandler<EditRecipientInfoCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly IUnitOfWork _unitOfWork;
            public EditRecipientInfoCommandHandler(IOrderService orderService, IUnitOfWork unitOfWork, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
                _orderLogService = orderLogService;
            }

            public async Task<RequestResult<bool>> Handle(EditRecipientInfoCommand command, CancellationToken cancellationToken)
            {
                var order = await _orderService.Queryable().FirstOrDefaultAsync(x => x.Id == command.OrderId);
                if (order == null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "not found");

                string note = "";
                if (order.RecipientFirstName != command.NewFirstName) note += $"RecipientFirstName changed from {order.RecipientFirstName} to {command.NewFirstName}\n";
                if (order.RecipientLastName != command.NewLastName) note += $"RecipientFirstName changed from {order.RecipientLastName} to {command.NewLastName}\n";
                if (order.PhoneNumber != command.NewPhoneNumber) note += $"RecipientFirstName changed from {order.PhoneNumber} to {command.NewPhoneNumber}\n";
                if (order.FullAddress != command.NewaAdderss) note += $"RecipientFirstName changed from {order.FullAddress} to {command.NewaAdderss}";

                order.RecipientFirstName = command.NewFirstName;
                order.RecipientLastName = command.NewLastName;
                order.PhoneNumber = command.NewPhoneNumber;
                order.FullAddress = command.NewaAdderss;

                _orderService.Update(order);
                await _orderLogService.OrderUpdatedLog(order, command.UserId, note);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "ERROR");
            }
        }
    }
}
