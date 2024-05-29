using EcommerceAPI.Data.Models;
using System.Text.Json;

namespace EcommerceAPI.Shared.ViewModels
{
    public class BaseItemVM
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

        public static BaseItemVM GenerateFromBaseItem(BaseItem baseItem)
        {
            try
            {
                var x = new BaseItemVM
                {
                    BaseItemType = baseItem.BaseItemType,
                    Id = baseItem.Id,
                    IsHomeOrCategory = baseItem.IsHomeOrCategory,
                    IsPhone = baseItem.IsPhone,
                    IsWeb = baseItem.IsWeb,
                    NameAr = baseItem.NameAr,
                    NameEn = baseItem.NameEn,
                    OrderIndex = baseItem.OrderIndex,
                };

                if (x.BaseItemType == BaseItemTypes.BrandsSet)
                    x.Brands = JsonSerializer.Deserialize<BaseItemBrand[]>(baseItem.Data) ?? [];
                else if (x.BaseItemType == BaseItemTypes.CategoriesSet)
                    x.Categories = JsonSerializer.Deserialize<BaseItemCategory[]>(baseItem.Data) ?? [];
                else if (x.BaseItemType == BaseItemTypes.SingleBanner)
                    x.SingleBanner = JsonSerializer.Deserialize<SingleBanner>(baseItem.Data) ?? new();
                else if (x.BaseItemType == BaseItemTypes.MultipleBanners)
                    x.MultipleBanners = JsonSerializer.Deserialize<BaseItemMultipleBanners>(baseItem.Data) ?? new();
                else if (x.BaseItemType == BaseItemTypes.ProductsSet)
                    x.Products = JsonSerializer.Deserialize<BaseItemBrandProduct[]>(baseItem.Data) ?? [];

                return x;
            }
            catch (Exception ex) { return null; }
        }
    }
}
