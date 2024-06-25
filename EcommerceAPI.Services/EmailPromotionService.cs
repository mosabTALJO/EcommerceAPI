using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;

namespace EcommerceAPI.Services
{
    public interface IEmailPromotionService : IService<EmailPromotion>
    {
    }

    public class EmailPromotionService : Service<EmailPromotion>, IEmailPromotionService
    {
        public EmailPromotionService(IRepository<EmailPromotion> repo) : base(repo)
        {

        }
    }
}
