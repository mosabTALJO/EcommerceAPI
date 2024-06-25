using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;

namespace EcommerceAPI.Services
{
    public interface IFrontendSettingsService : IService<FrontendSetting>
    {
    }
    public class FrontendSettingsService : Service<FrontendSetting>, IFrontendSettingsService
    {
        public FrontendSettingsService(IRepository<FrontendSetting> repository) : base(repository)
        {
        }
    }
}
