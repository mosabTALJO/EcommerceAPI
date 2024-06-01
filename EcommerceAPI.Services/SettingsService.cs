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

        public SettingsService(ITrackableRepository<Settings> repo, IUnitOfWork unitOfWork, MyDbContext mySqlContext) : base(repo)
        {
            this.unitOfWork = unitOfWork;
            _mySqlContext = mySqlContext;
        }

        public async Task CleanDb(List<int> categoryIDs)
        {
            List<int> failedIds = [];
            _mySqlContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(20));
            //remove all products from the related categories EXCEPT the prodcuts that ordered before
            foreach (var id in categoryIDs)
            {
                try
                {
                    await _mySqlContext.Database.ExecuteSqlAsync(@$"DELETE products FROM products 
                                                                       LEFT JOIN orderitem ON products.id = orderitem.productid 
                                                                       WHERE products.categoryid = {id} and  orderId IS NULL;");

                    await _mySqlContext.Database.ExecuteSqlAsync(@$"update products set IsActive=0 where categoryid = {id}");
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

        public async Task CleanDbWithBrandId(List<int> categoryIDs, int brandId)
        {
            List<int> failedIds = [];
            _mySqlContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(20));
            //remove all products from the related categories EXCEPT the prodcuts that ordered before
            foreach (var id in categoryIDs)
            {
                try
                {
                    await _mySqlContext.Database.ExecuteSqlAsync(@$"DELETE products FROM products 
                                                                       LEFT JOIN orderitem ON products.id = orderitem.productid 
                                                                       WHERE products.categoryid = {id} and  orderId IS NULL and products.brandid = {brandId};");

                    await _mySqlContext.Database.ExecuteSqlAsync(@$"update products set IsActive=0 where categoryid = {id} and brandid = {brandId}");
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
