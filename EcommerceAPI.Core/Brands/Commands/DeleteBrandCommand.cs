using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.Brands.Commands
{
    public class DeleteBrandCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, RequestResult<bool>>
        {
            private readonly IBrandService _brandService;
            public readonly IUnitOfWork _unitOfWork;
            public DeleteBrandCommandHandler(IBrandService brandService, IUnitOfWork unitOfWork)
            {
                _brandService = brandService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(DeleteBrandCommand command, CancellationToken cancellationToken)
            {
                if (await _brandService.FindAsync(command.Id) is Brand item)
                {
                    if (!_brandService.CanDelete(command.Id))
                        return new RequestResult<bool>(ActionResultType.BadRequest, "Unable to remove Brand because there is (at least) one linked product (or Category as parent) to it");
                    _brandService.Delete(item);
                    if (await _unitOfWork.SaveChangesAsync() > 0)
                        return new RequestResult<bool>(true);
                }
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
