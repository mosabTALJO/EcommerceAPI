using EcommerceAPI.Data.Models;
using EcommerceAPI.Entities.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services
{
    public interface IKargoCompanyService : IService<KargoCompany>
    {
        void CreateKargoCompany(KargoCompany company);
        void UpdateKargoCompany(KargoCompany company);
        void DeleteKargoCompany(KargoCompany company);
    }
    public class KargoCompanyService : Service<KargoCompany>, IKargoCompanyService
    {
        public KargoCompanyService(IRepository<KargoCompany> repo) : base(repo)
        {
        }

        public void CreateKargoCompany(KargoCompany company)
        {
            Insert(company);
        }
        public void UpdateKargoCompany(KargoCompany company)
        {
            Update(company);
        }
        public void DeleteKargoCompany(KargoCompany company)
        {
            Delete(company);
        }
    }
}
