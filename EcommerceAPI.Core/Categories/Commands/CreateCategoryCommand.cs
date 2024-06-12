using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<RequestResult<bool>>
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string NameTr { get; set; }
        public string Image { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; }
        public int? OrderIndex { get; set; }
        public ICollection<CreateCategoryCommand> Children { get; set; }
        public class PostCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, RequestResult<bool>>
        {
            private readonly ICategoryService _categoryService;
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;
            public PostCategoryCommandHandler(ICategoryService categoryService, IMapper mapper, IUnitOfWork unitOfWork)
            {
                _categoryService = categoryService;
                _mapper = mapper;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
            {
                var category = _mapper.Map<Category>(command);
                _categoryService.Insert(category);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(false, ActionResultType.BadRequest, "Error");
            }
        }
    }
}
