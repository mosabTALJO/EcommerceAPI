using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.Categories.Commands
{
    public class OrderCategoriesCommand : IRequest<RequestResult<bool>>
    {
        public CategoryOrdering[] Items { get; set; }
        public OrderCategoriesCommand(CategoryOrdering[] items)
        {
            Items = items;
        }

        public class OrderCategoriesCommandHandler : IRequestHandler<OrderCategoriesCommand, RequestResult<bool>>
        {
            private readonly ICategoryService _categoryService;
            private readonly IUnitOfWork _unitOfWork;

            public OrderCategoriesCommandHandler(IUnitOfWork unitOfWork, ICategoryService categoryService)
            {
                _unitOfWork = unitOfWork;
                _categoryService = categoryService;
            }

            public async Task<RequestResult<bool>> Handle(OrderCategoriesCommand command, CancellationToken cancellationToken)
            {
                foreach (var item in command.Items)
                {
                    var dbItem = _categoryService.Queryable().First(x => x.Id == item.CategoryId);
                    dbItem.OrderIndex = item.OrderIndex;
                    dbItem.Discount = item.Discount;

                    _categoryService.Update(dbItem);

                    foreach (var child in item.Children ?? new List<CategoryOrdering>())
                    {
                        var dbItem1 = _categoryService.Queryable().First(x => x.Id == child.CategoryId);
                        dbItem1.OrderIndex = child.OrderIndex;
                        dbItem1.Discount = child.Discount;

                        _categoryService.Update(dbItem1);

                        foreach (var childchild in child.Children ?? new List<CategoryOrdering>())
                        {
                            var dbItem2 = _categoryService.Queryable().First(x => x.Id == childchild.CategoryId);
                            dbItem2.OrderIndex = childchild.OrderIndex;
                            dbItem2.Discount = childchild.Discount;

                            _categoryService.Update(dbItem2);
                        }
                    }
                }

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(false, ActionResultType.BadRequest, "Error");
            }
        }
    }
}
