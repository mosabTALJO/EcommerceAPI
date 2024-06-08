using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.Coupons.Commands
{
    public class UpdateCouponCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string NameTr { get; set; }
        public DateTime ExpireDate { get; set; }
        public int Count { get; set; }
        public int RemainingCount { get; set; }
        public double Rate { get; set; }
        public bool IsActive { get; set; }
        public CouponType Type { get; set; }
        public int? TypeId { get; set; }
        public string TypeName { get; set; }
        public string TypeText => Type == CouponType.General ? "General" :
            Type == CouponType.Brand ? "Brand" : Type == CouponType.Category ? "Category" : "Product";
        public class UpdateCouponCommandHandler : IRequestHandler<UpdateCouponCommand, RequestResult<bool>>
        {
            private readonly ICouponService _couponService;
            private readonly IUnitOfWork _unitOfWork;
            public UpdateCouponCommandHandler(ICouponService couponService, IUnitOfWork unitOfWork)
            {
                _couponService = couponService;
                _unitOfWork = unitOfWork;
            }

            public async Task<RequestResult<bool>> Handle(UpdateCouponCommand command, CancellationToken cancellationToken)
            {
                if (await _couponService.FindAsync(command.Id) is Coupon c)
                {
                    if (c.DeletedAt.HasValue)
                        return new RequestResult<bool>(ActionResultType.BadRequest, "DeletedItem");

                    if (c.RemainingCount == c.Count)
                    {
                        c.Count = c.RemainingCount = command.Count;
                        c.Type = command.Type;
                        c.TypeId = command.TypeId;
                        c.Rate = command.Rate;
                    }

                    c.Code = command.Code;
                    c.NameAr = command.NameAr;
                    c.NameEn = command.NameEn;
                    c.NameTr = command.NameTr;
                    c.ExpireDate = command.ExpireDate;
                    c.IsActive = command.IsActive;

                    _couponService.Update(c);
                    if (await _unitOfWork.SaveChangesAsync() > 0)
                        return new RequestResult<bool>(true);
                }
                else
                    return new RequestResult<bool>(ActionResultType.NotFound, "IdError");
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
