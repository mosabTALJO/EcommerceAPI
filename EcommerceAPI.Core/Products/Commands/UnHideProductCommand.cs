using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Products.Commands
{
    public class UnHideProductCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public class UnHideProductCommandHandler : IRequestHandler<UnHideProductCommand, RequestResult<bool>>
        {
            private readonly IProductService _productService;
            private readonly IUnitOfWork _unitOfWork;

            public UnHideProductCommandHandler(IUnitOfWork unitOfWork, IProductService productService = null)
            {
                _unitOfWork = unitOfWork;
                _productService = productService;
            }
            public async Task<RequestResult<bool>> Handle(UnHideProductCommand command, CancellationToken cancellationToken)
            {
                var p = await _productService.Queryable().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == command.Id);
                if (p is null)
                    return new RequestResult<bool>(false, ActionResultType.NotFound);
                if (p.IsActive)
                    return new RequestResult<bool>(false, ActionResultType.BadRequest, "Product already unhidden");
                if (p.DeletedAt.HasValue)
                    return new RequestResult<bool>(false, ActionResultType.BadRequest, "DeletedItem");
                p.IsActive = true;
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(false, ActionResultType.BadRequest, "Error");
            }
        }
    }
}
