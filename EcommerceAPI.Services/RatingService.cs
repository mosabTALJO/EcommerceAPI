using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;

namespace EcommerceAPI.Services
{
    public interface IRatingService : IService<Rating>
    {
    }
    public class RatingService : Service<Rating>, IRatingService
    {
        public RatingService(ITrackableRepository<Rating> repo) : base(repo)
        {
        }
    }
}
