using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.FrontendSettings.Commands
{
    public class DeleteFrontendSettingCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public class DeleteFrontendSettingCommandHandler : IRequestHandler<DeleteFrontendSettingCommand, RequestResult<bool>>
        {
            private readonly IFrontendSettingsService _frontendSettingsService;
            private readonly IUnitOfWork _unitOfWork;
            public DeleteFrontendSettingCommandHandler(IFrontendSettingsService frontendSettingsService, IUnitOfWork unitOfWork)
            {
                _frontendSettingsService = frontendSettingsService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(DeleteFrontendSettingCommand command, CancellationToken cancellationToken)
            {
                var feSetting = await _frontendSettingsService.Queryable().FirstOrDefaultAsync(x => x.Id == command.Id);
                if (feSetting is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "IdError");

                _frontendSettingsService.Delete(feSetting);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
