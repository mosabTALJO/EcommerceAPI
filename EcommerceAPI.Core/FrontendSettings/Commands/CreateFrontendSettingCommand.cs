using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.FrontendSettings.Commands
{
    public class CreateFrontendSettingCommand : IRequest<RequestResult<int>>
    {
        public FrontendSettingType Type { get; set; }
        public string TextEn { get; set; }
        public string TextAr { get; set; }
        public class CreateFrontendSettingCommandHandler : IRequestHandler<CreateFrontendSettingCommand, RequestResult<int>>
        {
            private readonly IFrontendSettingsService _frontendSettingsService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public CreateFrontendSettingCommandHandler(IFrontendSettingsService frontendSettingsService, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _frontendSettingsService = frontendSettingsService;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }
            public async Task<RequestResult<int>> Handle(CreateFrontendSettingCommand command, CancellationToken cancellationToken)
            {
                var dbFESettings = _frontendSettingsService.Queryable().AsNoTracking().FirstOrDefault(x => x.Type == command.Type);
                if (dbFESettings != null)
                {
                    dbFESettings.TextEn = command.TextEn;
                    dbFESettings.TextAr = command.TextAr;

                    _frontendSettingsService.Update(dbFESettings);

                    if (await _unitOfWork.SaveChangesAsync() > 0)
                        return new RequestResult<int>(dbFESettings.Id);
                    return new RequestResult<int>(ActionResultType.BadRequest, "Error");
                }
                var frontendSetting = _mapper.Map<FrontendSetting>(command);
                _frontendSettingsService.Insert(frontendSetting);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<int>(frontendSetting.Id);
                return new RequestResult<int>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
