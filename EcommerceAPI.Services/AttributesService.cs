using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;

namespace EcommerceAPI.Services
{
    public interface IAttributesService : IService<ProductAttribute>
    {
        public IQueryable<ProductVariantAttribute> ProductVariantAttributes { get; }
    }

    public class AttributesService : Service<ProductAttribute>, IAttributesService
    {
        IRepository<ProductVariantAttribute> _productVariantAttribRepo;
        public AttributesService(IRepository<ProductAttribute> repo, IRepository<ProductVariantAttribute> productVariantAttribRepo) : base(repo)
        {
            _productVariantAttribRepo = productVariantAttribRepo;
        }
        public IQueryable<ProductVariantAttribute> ProductVariantAttributes => _productVariantAttribRepo.Queryable();
    }
}
