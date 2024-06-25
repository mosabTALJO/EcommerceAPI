using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;

namespace EcommerceAPI.Services
{
    public interface IProductVariantService : IService<ProductVariant>
    {
        
    }

    public class ProductVariantService : Service<ProductVariant>, IProductVariantService
    {
        public ProductVariantService(IRepository<ProductVariant> repository) : base(repository)
        {
        }
    }
}
