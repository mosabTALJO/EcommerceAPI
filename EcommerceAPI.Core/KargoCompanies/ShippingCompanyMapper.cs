using AutoMapper;
using EcommerceAPI.Core.ShippingCompanies.Command;
using EcommerceAPI.Entities.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.ShippingCompanies
{
    public class ShippingCompanyMapper : Profile
    {
        public ShippingCompanyMapper()
        {
            CreateMap<CreateShippingCompanyCommand, ShippingCompany>();
            CreateMap<ShippingCompany, ShippingCompanyVM>().ReverseMap();
        }
    }
}
