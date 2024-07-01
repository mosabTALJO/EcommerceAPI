using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using System.Text.Json;

namespace EcommerceAPI.Core.BaseItems.Command
{
    public class CreateBaseItemCommand : IRequest<RequestResult<int>>
    {
        public int IsHomeOrCategory { get; set; }//0 Home
        public BaseItemTypes BaseItemType { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameTr { get; set; } = string.Empty;
        public bool IsWeb { get; set; }
        public bool IsPhone { get; set; }
        public int OrderIndex { get; set; }

        public BaseItemCategory[]? Categories { get; set; }
        public BaseItemBrand[]? Brands { get; set; }
        public BaseItemBrandProduct[]? Products { get; set; }
        public SingleBanner? SingleBanner { get; set; }
        public BaseItemMultipleBanners? MultipleBanners { get; set; }
        public class CreateBaseItemCommandHandler : IRequestHandler<CreateBaseItemCommand, RequestResult<int>>
        {
            private readonly IBaseItemService _baseItemService;
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;

            public CreateBaseItemCommandHandler(IMapper mapper, IBaseItemService baseItemService, IUnitOfWork unitOfWork)
            {
                _mapper = mapper;
                _baseItemService = baseItemService;
                _unitOfWork = unitOfWork;
            }

            public async Task<RequestResult<int>> Handle(CreateBaseItemCommand command, CancellationToken cancellationToken)
            {
                //item.Id = 0;
                command.OrderIndex = _baseItemService.Queryable().OrderByDescending(x => x.OrderIndex).FirstOrDefault()?.OrderIndex ?? 0 + 1;
                var x = _mapper.Map<BaseItem>(command);

                if (x.BaseItemType == BaseItemTypes.BrandsSet)
                    x.Data = JsonSerializer.Serialize(command.Brands);
                else if (x.BaseItemType == BaseItemTypes.ProductsSet)
                    x.Data = JsonSerializer.Serialize(command.Products);
                else if (x.BaseItemType == BaseItemTypes.SingleBanner)
                    x.Data = JsonSerializer.Serialize(command.SingleBanner);
                else if (x.BaseItemType == BaseItemTypes.MultipleBanners)
                    x.Data = JsonSerializer.Serialize(command.MultipleBanners);
                else if (x.BaseItemType == BaseItemTypes.CategoriesSet)
                    x.Data = JsonSerializer.Serialize(command.Categories);

                _baseItemService.Insert(x);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<int>(x.Id);
                return new RequestResult<int>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
