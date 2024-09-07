using EcommerceAPI.Core.ShippmentCompanies.Command;
using EcommerceAPI.Core.ShippmentCompanies.Queries;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    [Authorize(Policy = PoliciesText.EditShippmentCompanies)]
    [ApiExplorerSettings(GroupName = "cp")]
    public class ShippmentCompanyController : MyControllerBase
    {
        public ShippmentCompanyController(IMediator mediator) : base(mediator: mediator)
        {
        }

        [HttpGet]
        public async Task<ActionResult<ListResponse<ShippmentCompanyVM>>> Get(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            return Ok(await _mediator.Send(new GetAllShippmentCompaniesnQuery { PageNumber = pageNumber, PageSize = pageSize }));
        }

        [HttpPost]
        public async Task<ActionResult<RequestResult<int>>> Post(CreateShippmentCompanyCommand command)
        {
            return GetActionResult(await _mediator.Send(command));
        }
        
        [HttpPut("{Id}")]
        public async Task<ActionResult<RequestResult<bool>>> Put(int Id, UpdateShippmentCompanyCommand command)
        {
            command.Id = Id;
            return GetActionResult(await _mediator.Send(command));
        }
        
        [HttpDelete("{Id}")]
        public async Task<ActionResult<RequestResult<bool>>> Delete(int Id)
        {
            return GetActionResult(await _mediator.Send(new DeleteShippmentCompanyCommand { Id = Id }));
        }
    }
}
