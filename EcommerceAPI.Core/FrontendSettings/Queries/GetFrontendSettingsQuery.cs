using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.FrontendSettings.Queries
{
    public class GetFrontendSettingsQuery : IRequest<RequestResult<FrontendSettingVM[]>>
    {
        public class GetFrontendSettingsQueryHandler : IRequestHandler<GetFrontendSettingsQuery, RequestResult<FrontendSettingVM[]>>
        {
            private readonly IFrontendSettingsService _frontendSettingsService;
            private readonly IMapper _mapper;
            public GetFrontendSettingsQueryHandler(IFrontendSettingsService frontendSettingsService, IMapper mapper)
            {
                _frontendSettingsService = frontendSettingsService;
                _mapper = mapper;
            }
            public async Task<RequestResult<FrontendSettingVM[]>> Handle(GetFrontendSettingsQuery query, CancellationToken cancellationToken)
            {
                return new RequestResult<FrontendSettingVM[]>(await _frontendSettingsService.Queryable().AsNoTracking().To<FrontendSettingVM>(_mapper).ToArrayAsync());
            }
        }
    }
}
