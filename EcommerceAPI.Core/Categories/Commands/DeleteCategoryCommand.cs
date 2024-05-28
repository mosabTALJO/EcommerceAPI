using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Categories.Commands
{
    public class DeleteCategoryCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, RequestResult<bool>>
        {
            private readonly ICategoryService _categoryService;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, ICategoryService categoryService)
            {
                _unitOfWork = unitOfWork;
                _categoryService = categoryService;
            }
            public async Task<RequestResult<bool>> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
            {
                if (await _categoryService.Queryable().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == command.Id) is Category item)
                {
                    if (!_categoryService.CanDelete(command.Id))
                        return new RequestResult<bool>(false, ActionResultType.BadRequest, "Unable to remove Category because there is (at least) one linked product to it");
                    _categoryService.Delete(item);
                    if (await _unitOfWork.SaveChangesAsync() > 0)
                        return new RequestResult<bool>(true);
                }
                return new RequestResult<bool>(false, ActionResultType.BadRequest, "Error");
            }
        }
    }
}
