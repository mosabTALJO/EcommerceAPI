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
        void CreateCountryData(Country c, City[] passedCities);
        void UpdateCountryData(Country c, City[] passedCities);
        void DeleteCountryData(Country c);
    }
    public class CountryService : Service<Country>, ICountryService
    {
        IRepository<City> _cityRepo;
        public CountryService(IRepository<Country> cRepo, IRepository<City> cityRepo) : base(cRepo)
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
        public void CreateCountryData(Country c, City[] passedCities)
        {
            foreach (var newItem in passedCities.ToArray())
            {
                newItem.Country = c;
                newItem.CountryId = c.Id;
                _cityRepo.Insert(newItem);
            }
            Insert(c);
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

        public void DeleteCountryData(Country c)
        {
            foreach (var newItem in c.Cities)
                _cityRepo.Delete(newItem);
            Delete(c);
        }
    }
}