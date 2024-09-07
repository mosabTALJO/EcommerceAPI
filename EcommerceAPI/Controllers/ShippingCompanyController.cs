using EcommerceAPI.Core.ShippingCompanies.Command;
using EcommerceAPI.Core.ShippingCompanies.Queries;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    [Authorize(Policy = PoliciesText.EditShippingCompanies)]
    [ApiExplorerSettings(GroupName = "cp")]
    public class ShippingCompaniesController : MyControllerBase
    {
        public ShippingCompaniesController(IMediator mediator) : base(mediator: mediator)
        {
        }

        [HttpGet]
        public async Task<ActionResult<ListResponse<ShippingCompanyVM>>> Get(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            return Ok(await _mediator.Send(new GetAllShippingCompaniesnQuery { PageNumber = pageNumber, PageSize = pageSize }));
        }

        [HttpPost]
        public async Task<ActionResult<RequestResult<int>>> Post(CreateShippingCompanyCommand command)
        {
            return GetActionResult(await _mediator.Send(command));
        }
        
        [HttpPut("{Id}")]
        public async Task<ActionResult<RequestResult<bool>>> Put(int Id, UpdateShippingCompanyCommand command)
        {
            command.Id = Id;
            return GetActionResult(await _mediator.Send(command));
        }
        
        [HttpDelete("{Id}")]
        public async Task<ActionResult<RequestResult<bool>>> Delete(int Id)
        {
            return GetActionResult(await _mediator.Send(new DeleteShippingCompanyCommand { Id = Id }));
        }
    }
}
