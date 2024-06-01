using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orderings.Commands
{
    public class CreateOrderingCommand : IRequest<RequestResult<bool>>
    {
        public OrderCategoryBrand[] Items { get; set; }
        public class CreateOrderingCommandHandler : IRequestHandler<CreateOrderingCommand, RequestResult<bool>>
        {
            private readonly IOrderCategoryBrandService _orderCategoryBrandService;
            private readonly IUnitOfWork _unitOfWork;

            public CreateOrderingCommandHandler(IOrderCategoryBrandService orderCategoryBrandService, IUnitOfWork unitOfWork)
            {
                _orderCategoryBrandService = orderCategoryBrandService;
                _unitOfWork = unitOfWork;
            }

            public async Task<RequestResult<bool>> Handle(CreateOrderingCommand command, CancellationToken cancellationToken)
            {
                var categoryId = command.Items.First().CategoryId;
                var lst = _orderCategoryBrandService.Queryable().AsNoTracking().Where(x => x.CategoryId == categoryId).ToArray();

                foreach (var item in command.Items)
                {
                    if (item.Id == 0)
                        _orderCategoryBrandService.Insert(item);
                    else if (lst.Any(x => x.Id == item.Id))
                        _orderCategoryBrandService.Update(item);
                    else
                        _orderCategoryBrandService.Delete(item);
                }
                return new RequestResult<bool>(await _unitOfWork.SaveChangesAsync() > 0);
            }
        }
    }
}
