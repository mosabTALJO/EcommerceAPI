using EcommerceAPI.Core.FrontendSettings.Commands;
using EcommerceAPI.Core.FrontendSettings.Queries;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    [ApiExplorerSettings(GroupName = "cp")]
    public class FrontEndSettingsController : MyControllerBase
    {
        public FrontEndSettingsController(IMediator mediator) : base(mediator: mediator)
        {
        }

        [HttpGet]
        public async Task<ActionResult<RequestResult<FrontendSettingVM[]>>> Get()
        {
            return GetActionResult(await _mediator.Send(new GetFrontendSettingsQuery()));
        }

        [HttpGet("{Type}")]
        public async Task<ActionResult<RequestResult<FrontendSettingVM>>> GetByType(int Type)
        {
            return GetActionResult(await _mediator.Send(new GetFrontendSettingByTypeQuery { Type = (FrontendSettingType)Type }));
        }

        [HttpPost]
        [Authorize(Policy = PoliciesText.EditFrontendSettings)]
        public async Task<ActionResult<RequestResult<int>>> Post([FromBody] CreateFrontendSettingCommand command)
        {
            return GetActionResult(await _mediator.Send(command));
        }

        [HttpPut("{Id}")]
        [Authorize(Policy = PoliciesText.EditFrontendSettings)]
        public async Task<ActionResult<RequestResult<bool>>> Put(int Id, [FromBody] UpdateFrontendSettingCommand command)
        {
            command.Id = Id;
            return GetActionResult(await _mediator.Send(command));
        }

        [HttpDelete("{Id}")]
        [Authorize(Policy = PoliciesText.EditFrontendSettings)]
        public async Task<ActionResult<RequestResult<bool>>> Delete(int Id, [FromBody] DeleteFrontendSettingCommand command)
        {
            command.Id = Id;
            return GetActionResult(await _mediator.Send(command));
        }
    }
}
