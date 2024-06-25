using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;

namespace EcommerceAPI.Services
{
    public interface IProductFavoriteService : IService<ProductFavorite>
    {

    }

    public class ProductFavoriteService : Service<ProductFavorite>, IProductFavoriteService
    {
        public ProductFavoriteService(IRepository<ProductFavorite> repo) : base(repo)
        {

        }
    }
}
