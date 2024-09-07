using AutoMapper;
using EcommerceAPI.Core.ShippmentCompanies.Command;
using EcommerceAPI.Entities.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.ShippmentCompanies
{
    public class ShippmentCompanyMapper : Profile
    {
        public ShippmentCompanyMapper()
        {
            CreateMap<CreateShippmentCompanyCommand, ShippmentCompany>();
            CreateMap<ShippmentCompany, ShippmentCompanyVM>().ReverseMap();
        }
    }
}
