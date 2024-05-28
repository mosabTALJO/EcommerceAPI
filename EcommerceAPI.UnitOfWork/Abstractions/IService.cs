namespace EcommerceAPI.UnitOfWork.Abstractions
{
    public interface IService<TEntity> : ITrackableRepository<TEntity> where TEntity : class, ITrackable
    {
    }
}