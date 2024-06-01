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
        public EmailPromotionService(ITrackableRepository<EmailPromotion> repo) : base(repo)
        {

        }
    }
}
