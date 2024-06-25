using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;

namespace EcommerceAPI.Services
{
    public interface IOrderCategoryBrandService : IService<OrderCategoryBrand>
    {

    }

    public class OrderCategoryBrandService : Service<OrderCategoryBrand>, IOrderCategoryBrandService
    {
        public OrderCategoryBrandService(IRepository<OrderCategoryBrand> repo) : base(repo)
        {
        }
    }
}
