using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Services;

namespace EcommerceAPI.Services
{
    public interface IBaseItemService : IService<BaseItem>
    {

    }

    public class BaseItemService : Service<BaseItem>, IBaseItemService
    {
        public BaseItemService(ITrackableRepository<BaseItem> repo) : base(repo)
        {

        }
    }
}
