using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.BaseItems.Command
{
    public class OrderBaseItemsCommand : IRequest<RequestResult<bool>>
    {
        public BaseItemVM[] Items { get; set; }
        public class OrderBaseItemsCommandHandler : IRequestHandler<OrderBaseItemsCommand, RequestResult<bool>>
        {
            private readonly IBaseItemService _baseItemService;
            private readonly IUnitOfWork _unitOfWork;

            public OrderBaseItemsCommandHandler(IBaseItemService baseItemService, IUnitOfWork unitOfWork)
            {
                _baseItemService = baseItemService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(OrderBaseItemsCommand command, CancellationToken cancellationToken)
            {
                foreach (var item in command.Items)
                {
                    var dbItem = _baseItemService.Queryable().First(x => x.Id == item.Id);
                    dbItem.OrderIndex = item.OrderIndex;
                    _baseItemService.Update(dbItem);
                }

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
