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
        IQueryable<Product> GetRelatedProducts(int id, string lang, double currencyRate);
        IQueryable<ProductType> GetProductTypes(int productId);
    }

    public class ProductService : Service<Product>, IProductService
    {
        ITrackableRepository<ProductType> _productTypeRepo;
        ITrackableRepository<OrderItem> _orderItemRepo;
        public ProductService(ITrackableRepository<Product> repo
            , ITrackableRepository<ProductType> productTypeRepo
            , ITrackableRepository<OrderItem> orderItemRepo) : base(repo)
        {
            _productTypeRepo = productTypeRepo;
            _orderItemRepo = orderItemRepo;
        }

        public void CreateProduct(Product p)
        {
            Insert(p);

            foreach (var pt in p.ProductTypes ?? new List<ProductType>())
            {
                pt.Product = p;
                //if (p.VendorId == Vendor.Custom)
                //    pt.StockLevel = 10;
                _productTypeRepo.Insert(pt);
            }
        }

        public void UpdateProduct(Product p)
        {
            var dbTypes = _productTypeRepo.Queryable().Where(x => x.ProductId == p.Id).ToList();
            foreach (var dbType in dbTypes)
            {
                var existed = p.ProductTypes.Where(x => x.Size == dbType.Size).ToList();
                if (!existed.Any())
                {
                    if (_orderItemRepo.Queryable().Any(oi => oi.ProductTypeId == dbType.Id))
                        dbType.StockLevel = 0;
                    else
                        _productTypeRepo.Delete(dbType);
                }
                else
                {
                    dbType.Barcode = existed[0].Barcode;
                    dbType.StockLevel = existed[0].StockLevel;
                    _productTypeRepo.Update(dbType);

                    p.ProductTypes = p.ProductTypes.Except(existed).ToList();
                }
            }
            foreach (var pt in p.ProductTypes)
            {
                pt.Product = p;
                //if (p.VendorId == Vendor.Custom)
                //    pt.StockLevel = 10;
                _productTypeRepo.Insert(pt);
            }
            Update(p);
        }

        public void DeleteProduct(Product p)
        {
            _productTypeRepo.Queryable().Where(x => x.ProductId == p.Id).ToList().ForEach(pt => _productTypeRepo.Delete(pt));
            Delete(p);
        }

        public int[] GetCategoryProductIds(List<int> categoryIds)
        {
            return Repository.Queryable().Where(x => x.CategoryId != null && categoryIds.Contains(x.CategoryId.Value)).Select(x => x.Id).ToArray();
        }

        public IQueryable<Product> GetRelatedProducts(int productId, string lang, double currencyRate)
        {
            var catId = Repository.Queryable().FirstOrDefault(x => x.Id == productId)?.CategoryId;
            if (catId is null)
                return null;

            var lst = Repository.Queryable().Include(x => x.Category)
                .Include(x => x.Brand).Where(x => !x.DeletedAt.HasValue);
            lst = lst.Where(x => x.Id != productId && x.CategoryId == catId).OrderBy(o => o.Id);

            var allCount = lst.Count();
            if (allCount > 5)
            {
                var skip = (int)(new Random().NextDouble() * allCount);
                lst = lst.Skip(skip).Take(10);
            }
            return lst;
        }

        public IQueryable<ProductType> GetProductTypes(int productId)
        {
            return _productTypeRepo.Queryable().Where(x => x.ProductId == productId);
        }
    }
}
