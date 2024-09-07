using EcommerceAPI.Entities.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;

namespace EcommerceAPI.Services
{
    public interface IShippingCompanyService : IService<ShippingCompany>
    {
        void CreateShippingCompany(ShippingCompany company);
        void UpdateShippingCompany(ShippingCompany company);
        void DeleteShippingCompany(ShippingCompany company);
    }
    public class ShippingCompanyService : Service<ShippingCompany>, IShippingCompanyService
    {
        public ShippingCompanyService(IRepository<ShippingCompany> repo) : base(repo)
        {
        }

        public void CreateShippingCompany(ShippingCompany company)
        {
            Insert(company);
        }
        public void UpdateShippingCompany(ShippingCompany company)
        {
            Update(company);
        }
        public void DeleteShippingCompany(ShippingCompany company)
        {
            Delete(company);
        }
    }
}
