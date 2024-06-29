using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services
{
    public interface IAttributesService : IService<ProductAttribute>
    {
        public Task<bool> HasRelatedVariants(int AttributeId);
        public Task<string[]> GetAttribiteValues(int AttributeId);
    }

    public class AttributesService : Service<ProductAttribute>, IAttributesService
    {
        IRepository<ProductVariantAttribute> _productVariantAttribRepo;
        public AttributesService(IRepository<ProductAttribute> repo, IRepository<ProductVariantAttribute> productVariantAttribRepo) : base(repo)
        {
            _productVariantAttribRepo = productVariantAttribRepo;
        }

        public async Task<string[]> GetAttribiteValues(int AttributeId) =>
            await _productVariantAttribRepo.Queryable().Where(x => x.ProductAttributeID == AttributeId).Select(x => x.Value).Distinct().ToArrayAsync();

        public async Task<bool> HasRelatedVariants(int AttributeId) =>
            await _productVariantAttribRepo.Queryable().AnyAsync(x => x.ProductAttributeID == AttributeId);
    }
}
