namespace EcommerceAPI.UnitOfWork.Abstractions
{
    public interface IService<TEntity> : IRepository<TEntity> where TEntity : class, ITrackable
    {
    }
}