using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;

namespace EcommerceAPI.Services
{
    public interface IUserAddressService : IService<UserAddress>
    {

    }

    public class UserAddressService : Service<UserAddress>, IUserAddressService
    {
        public UserAddressService(IRepository<UserAddress> repo) : base(repo)
        {

        }
    }
}
