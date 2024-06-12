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
        ITrackableRepository<ProductVariantAttribute> _productVariantAttribRepo;
        public AttributesService(ITrackableRepository<ProductAttribute> repo, ITrackableRepository<ProductVariantAttribute> productVariantAttribRepo) : base(repo)
        {
            _productVariantAttribRepo = productVariantAttribRepo;
        }
        public IQueryable<ProductVariantAttribute> ProductVariantAttributes => _productVariantAttribRepo.Queryable();
    }
}
