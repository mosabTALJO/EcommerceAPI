using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services
{
    public interface ISettingsService : IService<Settings>
    {
        public Task CleanDb(List<int> categoryIDs);
    }

    public class SettingsService : Service<Settings>, ISettingsService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly MyDbContext _mySqlContext;

        public SettingsService(IRepository<Settings> repo, IUnitOfWork unitOfWork, MyDbContext mySqlContext) : base(repo)
        {
            this.unitOfWork = unitOfWork;
            _mySqlContext = mySqlContext;
        }

        public async Task CleanDb(List<int> categoryIDs)
        {
            Dictionary<int, string> failedIds = [];
            _mySqlContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(20));
            //remove all products from the related categories EXCEPT the prodcuts that ordered before
            foreach (var id in categoryIDs)
            {
                try
                {
                    await _mySqlContext.Products
                        .Where(x => 
                                x.CategoryId == id &&
                                !_mySqlContext.OrderItem.Where(oi => oi.ProductVariant.Product.CategoryId == id).Select(oi => oi.ProductVariant.ProductId).Contains(x.Id))
                        .ExecuteDeleteAsync();

                    await _mySqlContext.Products
                        .Where(x => x.CategoryId == id)
                        .ExecuteUpdateAsync(x => x.SetProperty(x => x.IsActive, x => false));
                }
                catch (Exception ex) { failedIds.Add(id, ex.Message + ex.InnerException?.Message); }
            }

            var record = Repository.Queryable().FirstOrDefault(x => x.Name == "IsCleaning") ?? new Settings { Name = "", };
            record.Value = "0";
            Update(record);
            await unitOfWork.SaveChangesAsync();

            if (failedIds.Any())
                throw new Exception("These Ids failed: " + string.Join(", ", failedIds.Select(x => x.Key + " ex: " + x.Value)));
        }

        public async Task CleanDbWithBrandId(List<int> categoryIDs, int brandId)
        {
            List<int> failedIds = [];
            _mySqlContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(20));
            //remove all products from the related categories EXCEPT the prodcuts that ordered before
            foreach (var id in categoryIDs)
            {
                try
                {
                    await _mySqlContext.Products
                            .Where(x =>
                                    x.CategoryId == id && x.BrandId == brandId &&
                                    !_mySqlContext.OrderItem.Where(oi => oi.ProductVariant.Product.CategoryId == id && oi.ProductVariant.Product.BrandId == brandId).Select(oi => oi.ProductVariant.ProductId).Contains(x.Id))
                            .ExecuteDeleteAsync();

                    await _mySqlContext.Products
                        .Where(x => x.CategoryId == id && x.BrandId == brandId)
                        .ExecuteUpdateAsync(x => x.SetProperty(x => x.IsActive, x => false));
                }
                catch { failedIds.Add(id); }
            }

            var record = Repository.Queryable().FirstOrDefault(x => x.Name == "IsCleaning") ?? new Settings { Name = "", };
            record.Value = "0";
            Update(record);
            await unitOfWork.SaveChangesAsync();

            if (failedIds.Any())
                throw new Exception("These Ids failed: " + string.Join(", ", failedIds));
        }
    }
}
