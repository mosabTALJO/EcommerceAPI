using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Products.Commands
{
    public class HideProductCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public class HideProductCommandHandler : IRequestHandler<HideProductCommand, RequestResult<bool>>
        {
            private readonly IProductService _productService;
            private readonly IUnitOfWork _unitOfWork;

            public HideProductCommandHandler(IUnitOfWork unitOfWork, IProductService productService = null)
            {
                _unitOfWork = unitOfWork;
                _productService = productService;
            }
            public async Task<RequestResult<bool>> Handle(HideProductCommand command, CancellationToken cancellationToken)
            {
                var p = await _productService.Queryable().FirstOrDefaultAsync(x => x.Id == command.Id);
                if (p is null)
                    return new RequestResult<bool>(false, ActionResultType.NotFound);
                if (!p.IsActive)
                    return new RequestResult<bool>(false, ActionResultType.BadRequest, "Product already hidden");
                if (p.DeletedAt.HasValue)
                    return new RequestResult<bool>(false, ActionResultType.BadRequest, "DeletedItem");
                p.IsActive = false;
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(false, ActionResultType.BadRequest, "Error");
            }
        }
    }
}
