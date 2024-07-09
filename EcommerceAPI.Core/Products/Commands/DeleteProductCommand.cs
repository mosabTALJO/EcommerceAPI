using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.Products.Commands
{
    public class DeleteProductCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, RequestResult<bool>>
        {
            private readonly IProductService _productService;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteProductCommandHandler(IUnitOfWork unitOfWork, IProductService productService = null)
            {
                _unitOfWork = unitOfWork;
                _productService = productService;
            }

            public async Task<RequestResult<bool>> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
            {
                if (await _productService.FindAsync(command.Id) is Product item)
                {
                    if (item.DeletedAt.HasValue)
                        return new RequestResult<bool>(false, ActionResultType.BadRequest, "DeletedItem");

                    await _productService.DeleteProduct(item);
                    if (await _unitOfWork.SaveChangesAsync() > 0)
                        return new RequestResult<bool>(true);
                }
                return new RequestResult<bool>(false, ActionResultType.BadRequest, "Error");
            }
        }
    }
}
