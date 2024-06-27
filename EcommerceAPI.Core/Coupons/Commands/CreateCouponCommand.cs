using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.Coupons.Commands
{
    public class CreateCouponCommand : IRequest<RequestResult<int>>
    {
        public string Code { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string NameTr { get; set; } = string.Empty;
        public DateTime ExpireDate { get; set; }
        public int Count { get; set; }
        public int RemainingCount { get; set; }
        public double Rate { get; set; }
        public bool IsActive { get; set; }
        public CouponType Type { get; set; }
        public int? TypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string? CustomerId { get; set; }
        public string TypeText => Type == CouponType.General ? "General" :
            Type == CouponType.Brand ? "Brand" : Type == CouponType.Category ? "Category" : "Product";
        public bool IsForCustomerFirstOrder { get; set; }

        public class CreateCouponCommandHandler : IRequestHandler<CreateCouponCommand, RequestResult<int>>
        {
            private readonly ICouponService _couponService;
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;
            public CreateCouponCommandHandler(ICouponService couponService, IMapper mapper, IUnitOfWork unitOfWork)
            {
                _couponService = couponService;
                _mapper = mapper;
                _unitOfWork = unitOfWork;
            }

            public async Task<RequestResult<int>> Handle(CreateCouponCommand command, CancellationToken cancellationToken)
            {
                command.RemainingCount = command.Count;
                var coupon = _mapper.Map<Coupon>(command);
                _couponService.Insert(coupon);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<int>(coupon.Id);
                return new RequestResult<int>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
