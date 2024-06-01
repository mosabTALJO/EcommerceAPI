using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services
{
    public interface ICollectionService : IService<Collection>
    {
        void UpdateCollection(Collection c, CollectionItem[] passedItems);
        Dictionary<int, List<CollectionItem>> GetCollectionsByProducts(int[] productIds);
        Task<Dictionary<int, List<CollectionItem>>> GetCollectionsByProductVariantIds(int[] productIds);
    }

    public class CollectionService : Service<Collection>, ICollectionService
    {
        private ITrackableRepository<CollectionItem> _collectionItemRepos;
        private ITrackableRepository<ProductVariant> _productVariantRepos;
        public CollectionService(ITrackableRepository<Collection> repo, ITrackableRepository<CollectionItem> collectionItemRepos, ITrackableRepository<ProductVariant> productVariantRepos) : base(repo)
        {
            _collectionItemRepos = collectionItemRepos;
            _productVariantRepos = productVariantRepos;
        }
        public Dictionary<int, List<CollectionItem>> GetCollectionsByProducts(int[] productIds)
        {
            var onFreeCollections = Repository.Queryable()
                .Where(x => x.IsOneFree).Select(x => x.Id).ToArray();
            return _collectionItemRepos.Queryable()
                .Where(x => onFreeCollections.Contains(x.CollectionId))
                .Where(x => productIds.Contains(x.ProductId))
                .ToArray()
                .GroupBy(x => x.CollectionId)
                .ToDictionary(x => x.Key, x => x.ToList());
        }
        public void UpdateCollection(Collection c, CollectionItem[] passedItems)
        {
            var dbItems = _collectionItemRepos.Queryable().Where(x => x.CollectionId == c.Id).ToList();
            foreach (var dbItem in dbItems)
            {
                var item = passedItems.FirstOrDefault(x => x.Id == dbItem.Id);
                if (item != null)
                {
                    dbItem.OrderIndex = item.OrderIndex;
                    _collectionItemRepos.Update(dbItem);
                }
                else
                {
                    _collectionItemRepos.Delete(dbItem);
                }
            }
            foreach (var newItem in passedItems.Where(x => x.Id == 0).ToArray())
            {
                newItem.CollectionId = c.Id;
                _collectionItemRepos.Insert(newItem);
            }
            Update(c);
        }
        public async Task<Dictionary<int, List<CollectionItem>>> GetCollectionsByProductVariantIds(int[] productVariantIds)
        {
            var onFreeCollections = Repository.Queryable()
                .Where(x => x.IsOneFree).Select(x => x.Id).ToArray();
            var productIds = await _productVariantRepos.Queryable().Where(x => EF.Constant(productVariantIds).Contains(x.Id)).Select(x => x.ProductId).ToArrayAsync();
            return _collectionItemRepos.Queryable()
                .Where(x => onFreeCollections.Contains(x.CollectionId))
                .Where(x => productIds.Contains(x.ProductId))
                .ToArray()
                .GroupBy(x => x.CollectionId)
                .ToDictionary(x => x.Key, x => x.ToList());
        }
    }
}
