using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using System.Text.Json;

namespace EcommerceAPI.Core.BaseItems.Command
{
    public class UpdateBaseItemCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public int IsHomeOrCategory { get; set; }//0 Home
        public BaseItemTypes BaseItemType { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public bool IsWeb { get; set; }
        public bool IsPhone { get; set; }
        public int OrderIndex { get; set; }

        public BaseItemCategory[] Categories { get; set; }
        public BaseItemBrand[] Brands { get; set; }
        public BaseItemBrandProduct[] Products { get; set; }
        public SingleBanner SingleBanner { get; set; }
        public BaseItemMultipleBanners MultipleBanners { get; set; }
        public class UpdateBaseItemCommandHandler : IRequestHandler<UpdateBaseItemCommand, RequestResult<bool>>
        {
            private readonly IBaseItemService _baseItemService;
            private readonly IUnitOfWork _unitOfWork;

            public UpdateBaseItemCommandHandler(IBaseItemService baseItemService, IUnitOfWork unitOfWork)
            {
                _baseItemService = baseItemService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(UpdateBaseItemCommand command, CancellationToken cancellationToken)
            {
                var dbItem = _baseItemService.Queryable().FirstOrDefault(x => x.Id == command.Id);

                dbItem.BaseItemType = command.BaseItemType;
                dbItem.IsHomeOrCategory = command.IsHomeOrCategory;
                dbItem.IsWeb = command.IsWeb;
                dbItem.IsPhone = command.IsPhone;
                dbItem.NameAr = command.NameAr;
                dbItem.NameEn = command.NameEn;

                dbItem.Data = null;
                if (command.BaseItemType == BaseItemTypes.BrandsSet)
                    dbItem.Data = JsonSerializer.Serialize(command.Brands);
                else if (command.BaseItemType == BaseItemTypes.CategoriesSet)
                    dbItem.Data = JsonSerializer.Serialize(command.Categories);
                else if (command.BaseItemType == BaseItemTypes.ProductsSet)
                    dbItem.Data = JsonSerializer.Serialize(command.Products);
                else if (command.BaseItemType == BaseItemTypes.SingleBanner)
                    dbItem.Data = JsonSerializer.Serialize(command.SingleBanner);
                else if (command.BaseItemType == BaseItemTypes.MultipleBanners)
                    dbItem.Data = JsonSerializer.Serialize(command.MultipleBanners);

                _baseItemService.Update(dbItem);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
