using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.Brands.Commands
{
    public class CreateBrandCommand : IRequest<RequestResult<int>>
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string NameTr { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int? OrderIndex { get; set; } = null;
        public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, RequestResult<int>>
        {
            private readonly IBrandService _brandService;
            private readonly IMapper _mapper;
            public readonly IUnitOfWork _unitOfWork;
            public CreateBrandCommandHandler(IBrandService brandService, IMapper mapper, IUnitOfWork unitOfWork)
            {
                _brandService = brandService;
                _mapper = mapper;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<int>> Handle(CreateBrandCommand command, CancellationToken cancellationToken)
            {
                var brand = _mapper.Map<Brand>(command);
                if (!_brandService.IsValid(brand, out string error))
                    return new RequestResult<int>(0, ActionResultType.BadRequest, error);
                _brandService.Insert(brand);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<int>(brand.Id);
                return new RequestResult<int>(0, ActionResultType.BadRequest, "Error");
            }
        }
    }
}