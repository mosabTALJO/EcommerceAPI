using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.Countries
{
    public class CountryMapper : Profile
    {
        public CountryMapper()
        {
            CreateMap<Country, CountryVM>().ReverseMap();
            CreateMap<CityVM, City>().ReverseMap();
            CreateMap<CityForCountryUpdateVM, City>().ReverseMap();
        }
    }
}
