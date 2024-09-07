using EcommerceAPI.Entities.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;

namespace EcommerceAPI.Services
{
    public interface IShippmentCompanyService : IService<ShippmentCompany>
    {
        void CreateShippmentCompany(ShippmentCompany company);
        void UpdateShippmentCompany(ShippmentCompany company);
        void DeleteShippmentCompany(ShippmentCompany company);
    }
    public class ShippmentCompanyService : Service<ShippmentCompany>, IShippmentCompanyService
    {
        public ShippmentCompanyService(IRepository<ShippmentCompany> repo) : base(repo)
        {
        }

        public void CreateShippmentCompany(ShippmentCompany company)
        {
            Insert(company);
        }
        public void UpdateShippmentCompany(ShippmentCompany company)
        {
            Update(company);
        }
        public void DeleteShippmentCompany(ShippmentCompany company)
        {
            Delete(company);
        }
    }
}
