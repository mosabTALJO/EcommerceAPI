using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services
{
    public interface ICountryService : IService<Country>
    {
        Task<List<Country>> GetCountries();
        Country GetCountry(string code);
        void UpdateCountryData(Country c, City[] passedCities);
        bool UnBlockCity(int cityId);
    }
    public class CountryService : Service<Country>, ICountryService
    {
        ITrackableRepository<City> _cityRepo;
        public CountryService(ITrackableRepository<Country> cRepo, ITrackableRepository<City> cityRepo) : base(cRepo)
        {
            _cityRepo = cityRepo;
        }
        public async Task<List<Country>> GetCountries()
        {
            var countries = Repository.Queryable().AsNoTracking();
            return await countries.ToListAsync();
        }
        public Country GetCountry(string code)
        {
            var c = Repository.Queryable().AsNoTracking().FirstOrDefault(x => x.Code == code);

            return c;
        }
        public bool UnBlockCity(int cityId)
        {
            var city = _cityRepo.Queryable().FirstOrDefault(x => x.Id == cityId);
            if (city != null)
            {
                city.IsBlackList = false;
                return true;
            }

            return false;
        }
        public void UpdateCountryData(Country c, City[] passedCities)
        {
            var dbItems = _cityRepo.Queryable().Where(x => x.CountryId == c.Id).ToList();
            foreach (var dbItem in dbItems)
            {
                var item = passedCities.FirstOrDefault(x => x.Id == dbItem.Id);
                if (item != null)
                {
                    dbItem.NameEn = item.NameEn;
                    dbItem.NameAr = item.NameAr;
                    dbItem.IsBlackList = item.IsBlackList;
                    _cityRepo.Update(dbItem);
                }
                else
                {
                    _cityRepo.Delete(dbItem);
                }
            }
            foreach (var newItem in passedCities.Where(x => x.Id == 0).ToArray())
            {
                newItem.CountryId = c.Id;
                _cityRepo.Insert(newItem);
            }
            Update(c);
        }
    }
}