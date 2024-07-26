using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.ProductFavorites.Commands
{
    public class DeleteFavoriteProductCommand : IRequest<RequestResult<bool>>
    {
        public int ProductID { get; set; }
        public string UserId { get; set; }
        public class DeleteFavoriteProductCommandHandler : IRequestHandler<DeleteFavoriteProductCommand, RequestResult<bool>>
        {
            private readonly IProductFavoriteService _productFavoriteService;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteFavoriteProductCommandHandler(IProductFavoriteService productFavoriteService, IUnitOfWork unitOfWork)
            {
                _productFavoriteService = productFavoriteService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(DeleteFavoriteProductCommand command, CancellationToken cancellationToken)
            {
                var item = await _productFavoriteService.Queryable().FirstOrDefaultAsync(x => x.AppUserId == command.UserId && x.ProductId == command.ProductID);
                if (item is null)
                    return new RequestResult<bool>(ActionResultType.NotFound);

                _productFavoriteService.Delete(item);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
