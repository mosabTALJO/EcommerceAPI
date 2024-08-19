using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.BaseItems
{
    public class BaseItemCleaner
    {
        public static void CleanItems(BaseItemVM item, IMapper _mapper, IBrandService _brandService, ICategoryService _categoryService, IProductService _productService, ICurrencyRateService _currencyRateService, string toCurrency, string lang)
        {
            if (item.BaseItemType == BaseItemTypes.BrandsSet)
            {
                var brandsIds = item.Brands.Select(x => x.Id).ToArray();
                var dbBrands = _brandService.Queryable().Where(x => brandsIds.Contains(x.Id)).To<BrandVM>(_mapper).ToArray();
                for (int i = 0; i < item.Brands.Length; i++)
                {
                    var brand = item.Brands[i];
                    var dbBrand = dbBrands.FirstOrDefault(x => x.Id == brand.Id);
                    brand.Id = dbBrand?.Id ?? 0;
                    brand.Name = lang == "en" ? dbBrand?.NameEn : dbBrand?.NameAr;
                    brand.Image = dbBrand?.Icon;
                }
                item.Brands = item.Brands.Where(x => x.Id > 0).ToArray();
            }
            else if (item.BaseItemType == BaseItemTypes.CategoriesSet)
            {
                var categoryIds = item.Categories.Select(x => x.Id).ToArray();
                var dbCategories = _categoryService.Queryable().Where(x => categoryIds.Contains(x.Id)).To<CategoryVM>(_mapper).ToArray();
                for (int i = 0; i < item.Categories.Length; i++)
                {
                    var category = item.Categories[i];
                    var dbCategory = dbCategories.FirstOrDefault(x => x.Id == category.Id);
                    category.Id = dbCategory?.Id ?? 0;
                    category.Name = lang == "en" ? dbCategory?.NameEn : dbCategory?.NameAr;
                    category.Image = dbCategory?.Image;
                }
                item.Categories = item.Categories.Where(x => x.Id > 0).ToArray();
            }
            else if (item.BaseItemType == BaseItemTypes.ProductsSet)
            {
                var usdRate = 1 / _currencyRateService.GetUSDRate(toCurrency);

                var productsIds = item.Products.Select(x => x.Id).ToArray();
                item.Products = _productService.Queryable().Include(x => x.Category).Where(x => productsIds.Contains(x.Id)).Select(x => new BaseItemBrandProduct
                {
                    Name = lang == "en" ? x.NameEn : x.NameAr,
                    BrandName = lang == "en" ? x.Brand.NameEn : x.Brand.NameAr,
                    Id = x.Id,
                    MainImage = x.MainImage,
                    SalesPrice = usdRate * x.SalesPriceUSD,
                    Discount = x.Discount > 0 ? x.Discount : x.Category.Discount > 0 ? x.Category.Discount : null,
                }).ToArray();
            }
        }
    }
}
