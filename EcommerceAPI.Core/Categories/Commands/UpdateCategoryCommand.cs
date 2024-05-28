using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Categories.Commands
{
    public class UpdateCategoryCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string NameTr { get; set; }
        public string Image { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; }
        public int? OrderIndex { get; set; }
        public ICollection<UpdateCategoryCommand> Children { get; set; }
        public class PutCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, RequestResult<bool>>
        {
            private readonly ICategoryService _categoryService;
            private readonly IUnitOfWork _unitOfWork;

            public PutCategoryCommandHandler(IUnitOfWork unitOfWork, ICategoryService categoryService)
            {
                _unitOfWork = unitOfWork;
                _categoryService = categoryService;
            }

            public async Task<RequestResult<bool>> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
            {
                if (await _categoryService.Queryable().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == command.Id) is Category b)
                {
                    b.NameAr = command.NameAr;
                    b.NameEn = command.NameEn;
                    b.NameTr = command.NameTr;
                    b.Image = command.Image;
                    b.ParentId = command.ParentId;
                    b.IsActive = command.IsActive;
                    b.OrderIndex = command.OrderIndex;

                    _categoryService.Update(b);
                    if (await _unitOfWork.SaveChangesAsync() > 0)
                        return new RequestResult<bool>(true);
                }
                else
                    return new RequestResult<bool>(false, ActionResultType.NotFound, "IdError");
                return new RequestResult<bool>(false, ActionResultType.BadRequest, "Error");
            }
        }
    }
}
