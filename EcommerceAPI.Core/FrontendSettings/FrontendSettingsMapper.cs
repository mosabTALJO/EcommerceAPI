using AutoMapper;
using EcommerceAPI.Core.FrontendSettings.Commands;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.FrontendSettings
{
    public class FrontendSettingsMapper : Profile
    {
        public FrontendSettingsMapper()
        {
            CreateMap<CreateFrontendSettingCommand, FrontendSetting>();
            CreateMap<FrontendSettingVM, FrontendSetting>().ReverseMap();
        }
    }
}
