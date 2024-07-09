using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services
{
    public interface IProductService : IService<Product>
    {
        void CreateProduct(Product p);
        Task UpdateProduct(Product p);
        Task DeleteProduct(Product p);
        IQueryable<Product>? GetRelatedProducts(int id, string lang, double currencyRate);
        IQueryable<ProductVariant> GetProductVariant(int productId);
    }

    public class ProductService : Service<Product>, IProductService
    {
        IRepository<ProductVariant> _productVariantRepo;
        IRepository<OrderItem> _orderItemRepo;
        IRepository<ProductVariantAttribute> _productVariantAttributeRepo;
        public ProductService(IRepository<Product> repo,
            IRepository<ProductVariant> productVariantRepo,
            IRepository<OrderItem> orderItemRepo,
            IRepository<ProductVariantAttribute> productVariantAttributeRepo) : base(repo)
        {
            _productVariantRepo = productVariantRepo;
            _orderItemRepo = orderItemRepo;
            _productVariantAttributeRepo = productVariantAttributeRepo;
        }
        public void CreateProduct(Product p)
        {
            Insert(p);
            foreach (var v in p.ProductVariants ?? [])
            {
                v.Product = p;
                _productVariantRepo.Insert(v);
                foreach (var a in v.Attributes)
                {
                    a.ProductVariant = v;
                    _productVariantAttributeRepo.Insert(a);
                }
            }
        }
        public async Task UpdateProduct(Product p)
        {
            var dbVariants = _productVariantRepo.Queryable().Where(x => x.ProductId == p.Id).ToList();
            foreach (var dbVariant in dbVariants)
            {
                var variant = p.ProductVariants?.FirstOrDefault(x => x.Id == dbVariant.Id);
                if (variant == null)
                {
                    if (_orderItemRepo.Queryable().Any(oi => oi.ProductVariantId == dbVariant.Id))
                        dbVariant.StockLevel = 0;
                    else
                    {
                        _productVariantRepo.Delete(dbVariant);
                        await _productVariantAttributeRepo.Queryable().Where(x => x.ProductVariantID == dbVariant.Id).ExecuteDeleteAsync();
                    }
                }
                else
                {
                    dbVariant.Name = variant.Name;
                    dbVariant.SalesPrice = variant.SalesPrice;
                    dbVariant.Discount = variant.Discount;
                    dbVariant.MainImage = variant.MainImage;
                    dbVariant.Images = variant.Images;
                    dbVariant.SKU = variant.SKU;
                    dbVariant.FullDescription = variant.FullDescription;
                    dbVariant.IsActive = variant.IsActive;
                    dbVariant.Barcode = variant.Barcode;
                    dbVariant.StockLevel = variant.StockLevel;
                    _productVariantRepo.Update(dbVariant);
                }
            }
            foreach (var pv in p.ProductVariants?.Where(x => x.Id == 0) ?? [])
            {
                pv.ProductId = p.Id;
                _productVariantRepo.Insert(pv);
                foreach (var a in pv.Attributes)
                {
                    a.ProductVariant = pv;
                    _productVariantAttributeRepo.Insert(a);
                }
            }
            Update(p);
        }
        public async Task DeleteProduct(Product p)
        {
            var variantIds = p.ProductVariants?.Select(x => x.Id).ToArray();
            await _productVariantRepo.Queryable().Where(x => x.ProductId == p.Id).ExecuteDeleteAsync();
            if (variantIds?.Length > 0)
                await _productVariantAttributeRepo.Queryable().Where(x => EF.Constant(variantIds).Contains(x.ProductVariantID)).ExecuteDeleteAsync();
            Delete(p);
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