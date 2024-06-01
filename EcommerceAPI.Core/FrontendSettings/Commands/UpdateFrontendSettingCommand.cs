using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.FrontendSettings.Commands
{
    public class UpdateFrontendSettingCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public string TextEn { get; set; }
        public string TextAr { get; set; }
        public class UpdateFrontendSettingCommandHandler : IRequestHandler<UpdateFrontendSettingCommand, RequestResult<bool>>
        {
            private readonly IFrontendSettingsService _frontendSettingsService;
            private readonly IUnitOfWork _unitOfWork;
            public UpdateFrontendSettingCommandHandler(IFrontendSettingsService frontendSettingsService, IUnitOfWork unitOfWork)
            {
                _frontendSettingsService = frontendSettingsService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(UpdateFrontendSettingCommand command, CancellationToken cancellationToken)
            {
                var feSetting = await _frontendSettingsService.Queryable().FirstOrDefaultAsync(x => x.Id == command.Id);
                if (feSetting is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "IdError");

                feSetting.TextEn = command.TextEn;
                feSetting.TextAr = command.TextAr;

                _frontendSettingsService.Update(feSetting);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
