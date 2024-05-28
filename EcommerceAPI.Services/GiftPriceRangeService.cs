using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;

namespace EcommerceAPI.Services
{
    public interface IGiftPriceRangeService : IService<GiftPriceRange>
    {
        Task CreateGiftPriceRange(GiftPriceRange giftPriceRange);
        Task UpdateGiftPriceRange(GiftPriceRange giftPriceRange, List<Gift> newGifts);
        Task DeleteGiftPriceRange(GiftPriceRange giftPriceRange);
    }
    public class GiftPriceRangeService : Service<GiftPriceRange>, IGiftPriceRangeService
    {
        public ITrackableRepository<Gift> _gRepo { get; }
        public GiftPriceRangeService(ITrackableRepository<GiftPriceRange> repo, ITrackableRepository<Gift> gRepo) : base(repo)
        {
            _gRepo = gRepo;
        }

        public async Task CreateGiftPriceRange(GiftPriceRange giftPriceRange)
        {
            if (giftPriceRange != null)
            {
                foreach (var gift in giftPriceRange.Gifts)
                {
                    gift.GiftPriceRange = giftPriceRange;
                    gift.GiftPriceRangeId = giftPriceRange.Id;
                    _gRepo.Insert(gift);
                }
            }
            Insert(giftPriceRange);
        }

        public async Task UpdateGiftPriceRange(GiftPriceRange giftPriceRange, List<Gift> newGifts)
        {
            var dbGifts = _gRepo.Queryable().Where(g => g.GiftPriceRangeId == giftPriceRange.Id);
            foreach (var dbGift in dbGifts)
            {
                if (!newGifts.Any(x => x.Id == dbGift.Id))
                {
                    giftPriceRange.Gifts.Remove(dbGift);
                    _gRepo.Delete(dbGift);
                }

                else
                {
                    dbGift.Image = newGifts.FirstOrDefault(g => g.Id == dbGift.Id).Image;
                    dbGift.NameAr = newGifts.FirstOrDefault(g => g.Id == dbGift.Id).NameAr;
                    dbGift.NameEn = newGifts.FirstOrDefault(g => g.Id == dbGift.Id).NameEn;
                    dbGift.IsActive = newGifts.FirstOrDefault(g => g.Id == dbGift.Id).IsActive;
                    _gRepo.Update(dbGift);
                }
            }

            foreach (var gift in newGifts)
            {
                if (!dbGifts.Any(x => x.Id == gift.Id))
                {
                    giftPriceRange.Gifts.Add(gift);
                    _gRepo.Insert(gift);
                }
            }
            Update(giftPriceRange);
        }

        public async Task DeleteGiftPriceRange(GiftPriceRange giftPriceRange)
        {
            var dbGifts = _gRepo.Queryable().Where(g => g.GiftPriceRangeId == giftPriceRange.Id);
            foreach (var gift in dbGifts)
            {
                _gRepo.Delete(gift);
            }
            Delete(giftPriceRange);
        }
    }
}
