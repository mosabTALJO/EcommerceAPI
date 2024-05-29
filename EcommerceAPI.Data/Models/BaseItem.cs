using EcommerceAPI.Data.Helpers;

namespace EcommerceAPI.Data.Models
{
    public class BaseItem : TrackableEntity
    {
        public int Id { get; set; }
        public int IsHomeOrCategory { get; set; }//0 Home
        public BaseItemTypes BaseItemType { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public bool IsWeb { get; set; }
        public bool IsPhone { get; set; }
        public int OrderIndex { get; set; }

        //json helper that contains the data based on the BaseType
        public string Data { get; set; }
    }
    public enum BaseItemTypes { ProductsSet, BrandsSet, CategoriesSet, SingleBanner, MultipleBanners }


    public class BaseItemCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
    }
    public class BaseItemBrand
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
    }
    public class BaseItemBrandProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BrandName { get; set; }
        public double SalesPrice { get; set; }
        public double? Discount { get; set; }
        public double FinalSalesPrice => Math.Round(SalesPrice - SalesPrice * (Discount ?? 0) / 100, 1);
        public string MainImageRow { get; set; }
        public string MainImage => string.IsNullOrWhiteSpace(MainImageRow) || MainImageRow.StartsWith("https") ? MainImageRow : $"https://cdn.dsmcdn.com/mnresize/{600}/{500}" + MainImageRow;
        public bool IsFavorite { get; set; }
    }
    public class BaseItemMultipleBanners
    {
        public SingleBanner[] Banners { get; set; }
        public bool IsFullScreen { get; set; }
    }
    public class SingleBanner
    {
        public string Image { get; set; }
        public string DirectionUrl => Direction.ProductId.HasValue || Direction.CategoryId.HasValue || Direction.BrandId.HasValue || Direction.CollectionId.HasValue ?
            "https://app.trgatestore.com/Products" + (Direction.ProductId.HasValue ? $"/{Direction.ProductId}" : $"?categoryid={Direction.CategoryId}&brandid={Direction.BrandId}&collectionid={Direction.CollectionId}") : null;
        public BannerDirection Direction { get; set; }
    }
    public class BannerDirection
    {
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public int? CollectionId { get; set; }
        public int? ProductId { get; set; }
    }
}
