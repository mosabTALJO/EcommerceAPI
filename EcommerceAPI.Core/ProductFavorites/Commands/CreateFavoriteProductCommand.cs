using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.ProductFavorites.Commands
{
    public class CreateFavoriteProductCommand : IRequest<RequestResult<bool>>
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public class CreateFavoriteProductCommandHandler : IRequestHandler<CreateFavoriteProductCommand, RequestResult<bool>>
        {
            private readonly IProductFavoriteService _productFavoriteService;
            private readonly IUnitOfWork _unitOfWork;

            public CreateFavoriteProductCommandHandler(IProductFavoriteService productFavoriteService, IUnitOfWork unitOfWork)
            {
                _productFavoriteService = productFavoriteService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(CreateFavoriteProductCommand command, CancellationToken cancellationToken)
            {
                if (_productFavoriteService.Queryable().Any(x => x.AppUserId == command.UserId && x.ProductId == command.ProductId))
                    return new RequestResult<bool>(true);

                _productFavoriteService.Insert(new ProductFavorite { AppUserId = command.UserId, ProductId = command.ProductId });
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
