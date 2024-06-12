using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.FrontendSettings.Queries
{
    public class GetFrontendSettingByTypeQuery : IRequest<RequestResult<FrontendSettingVM>>
    {
        public FrontendSettingType Type { get; set; }
        public class GetFrontendSettingByTypeQueryHandler : IRequestHandler<GetFrontendSettingByTypeQuery, RequestResult<FrontendSettingVM>>
        {
            private readonly IFrontendSettingsService _frontendSettingsService;
            private readonly IMapper _mapper;
            public GetFrontendSettingByTypeQueryHandler(IFrontendSettingsService frontendSettingsService, IMapper mapper)
            {
                _frontendSettingsService = frontendSettingsService;
                _mapper = mapper;
            }
            public async Task<RequestResult<FrontendSettingVM>> Handle(GetFrontendSettingByTypeQuery query, CancellationToken cancellationToken)
            {
                var result = await _frontendSettingsService.Queryable().AsNoTracking().To<FrontendSettingVM>(_mapper).FirstOrDefaultAsync(x => x.Type == query.Type);
                if (result == null)
                    return new RequestResult<FrontendSettingVM>(new FrontendSettingVM { Type = query.Type });
                return new RequestResult<FrontendSettingVM>(result);
            }
        }
    }
}
