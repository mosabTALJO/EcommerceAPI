using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services
{
    public interface IProductService : IService<Product>
    {
        void CreateProduct(Product p);
        void UpdateProduct(Product p);
        void DeleteProduct(Product p);
        int[] GetCategoryProductIds(List<int> categoryIds);
        IQueryable<Product>? GetRelatedProducts(int id, string lang, double currencyRate);
        IQueryable<ProductVariant> GetProductVariant(int productId);
    }

    public class ProductService : Service<Product>, IProductService
    {
        ITrackableRepository<ProductVariant> _productVariantRepo;
        ITrackableRepository<OrderItem> _orderItemRepo;
        public ProductService(ITrackableRepository<Product> repo
            , ITrackableRepository<ProductVariant> productVariantRepo
            , ITrackableRepository<OrderItem> orderItemRepo) : base(repo)
        {
            _productVariantRepo = productVariantRepo;
            _orderItemRepo = orderItemRepo;
        }

        public void CreateProduct(Product p)
        {
            Insert(p);

            foreach (var pt in p.ProductVariants ?? new List<ProductVariant>())
            {
                pt.Product = p;
                //if (p.VendorId == Vendor.Custom)
                //    pt.StockLevel = 10;
                _productVariantRepo.Insert(pt);
            }
        }

        public void UpdateProduct(Product p)
        {
            var dbVariants = _productVariantRepo.Queryable().Where(x => x.ProductId == p.Id).ToList();
            foreach (var dbVariant in dbVariants)
            {
                var variant = p.ProductVariants.FirstOrDefault(x => x.Id == dbVariant.Id);
                if (variant == null)
                {
                    if (_orderItemRepo.Queryable().Any(oi => oi.ProductVariantId == dbVariant.Id))
                        dbVariant.StockLevel = 0;
                    else
                        _productVariantRepo.Delete(dbVariant);
                }
                else
                {
                    dbVariant.Barcode = variant.Barcode;
                    dbVariant.StockLevel = variant.StockLevel;
                    _productVariantRepo.Update(dbVariant);
                    //p.ProductVariants.Remove(variant);
                }
            }
            foreach (var pv in p.ProductVariants.Where(x => x.Id == 0))
            {
                pv.Product = p;
                _productVariantRepo.Insert(pv);
            }
            Update(p);
        }
        public void DeleteProduct(Product p)
        {
            _productVariantRepo.Queryable().Where(x => x.ProductId == p.Id).ExecuteDeleteAsync();
            Delete(p);
        }
        public int[] GetCategoryProductIds(List<int> categoryIds)
        {
            return Repository.Queryable().Where(x => x.CategoryId != null && categoryIds.Contains(x.CategoryId.Value)).Select(x => x.Id).ToArray();
        }
        public IQueryable<Product>? GetRelatedProducts(int productId, string lang, double currencyRate)
        {
            var catId = Repository.Queryable().Where(x => x.Id == productId).Select(x => x.CategoryId).FirstOrDefault();
            if (catId == null)
                return null;

            var lst = Repository.Queryable().Include(x => x.Category)
                .Include(x => x.Brand).Where(x => !x.DeletedAt.HasValue);
            lst = lst.Where(x => x.Id != productId && x.CategoryId == catId.Value).OrderBy(o => o.Id);

            var allCount = lst.Count();
            if (allCount > 5)
            {
                var skip = (int)(new Random().NextDouble() * allCount);
                lst = lst.Skip(skip).Take(10);
            }
            return lst;
        }
        public IQueryable<ProductVariant> GetProductVariant(int productId)
        {
            return _productVariantRepo.Queryable().Where(x => x.ProductId == productId);
        }
    }
}
