using AutoMapper;
using EcommerceAPI.Core.KargoCompanies.Command;
using EcommerceAPI.Entities.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.KargoCompanies
{
    public class KargoCompanyMapper : Profile
    {
        public KargoCompanyMapper()
        {
            CreateMap<CreateKargoCompanyCommand, KargoCompany>();
            CreateMap<KargoCompany, KargoCompanyVM>().ReverseMap();
        }
    }
}
