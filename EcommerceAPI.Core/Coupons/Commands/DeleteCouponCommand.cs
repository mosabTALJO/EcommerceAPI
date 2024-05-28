using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.Coupons.Commands
{
    public class DeleteCouponCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public class DeleteCouponCommandHandler : IRequestHandler<DeleteCouponCommand, RequestResult<bool>>
        {
            private readonly ICouponService _couponService;
            private readonly IUnitOfWork _unitOfWork;
            public DeleteCouponCommandHandler(ICouponService couponService, IUnitOfWork unitOfWork)
            {
                _couponService = couponService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(DeleteCouponCommand command, CancellationToken cancellationToken)
            {
                if (await _couponService.FindAsync(command.Id) is Coupon item)
                {
                    if (item.DeletedAt.HasValue)
                        return new RequestResult<bool>(ActionResultType.BadRequest, "DeletedItem");

                    _couponService.Delete(item);
                    if (await _unitOfWork.SaveChangesAsync() > 0)
                        return new RequestResult<bool>(true);
                }
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
