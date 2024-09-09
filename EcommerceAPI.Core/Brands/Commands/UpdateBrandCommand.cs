using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.Brands.Commands
{
    public class UpdateBrandCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string? NameTr { get; set; } = string.Empty;  
        public string Image { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int? OrderIndex { get; set; }
        public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, RequestResult<bool>>
        {
            private readonly IBrandService _brandService;
            private readonly IMapper _mapper;
            public readonly IUnitOfWork _unitOfWork;
            public UpdateBrandCommandHandler(IBrandService brandService, IMapper mapper, IUnitOfWork unitOfWork)
            {
                _brandService = brandService;
                _mapper = mapper;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(UpdateBrandCommand command, CancellationToken cancellationToken)
            {
                var b = await _brandService.FindAsync(command.Id);
                if (b is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "IdError");
                if (!_brandService.IsValid(_mapper.Map<Brand>(command), out string error))
                    return new RequestResult<bool>(ActionResultType.BadRequest, error);
                b.NameAr = command.NameAr?.Trim();
                b.NameEn = command.NameEn?.Trim();
                b.NameTr = command.NameTr?.Trim() ?? b.NameEn;
                b.Image = command.Image;
                b.Icon = command.Icon;
                b.IsActive = command.IsActive;
                b.OrderIndex = command.OrderIndex;
                _brandService.Update(b);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
