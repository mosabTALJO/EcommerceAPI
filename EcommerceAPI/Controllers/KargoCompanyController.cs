using EcommerceAPI.Core.KargoCompanies.Command;
using EcommerceAPI.Core.KargoCompanies.Queries;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    [Authorize(Policy = PoliciesText.EditKargoCompanies)]
    [ApiExplorerSettings(GroupName = "cp")]
    public class KargoCompanyController : MyControllerBase
    {
        public KargoCompanyController(IMediator mediator) : base(mediator: mediator)
        {
        }

        [HttpGet]
        public async Task<ActionResult<ListResponse<KargoCompanyVM>>> Get(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            return Ok(await _mediator.Send(new GetAllKargoCompaniesnQuery { PageNumber = pageNumber, PageSize = pageSize }));
        }

        [HttpPost]
        public async Task<ActionResult<RequestResult<int>>> Post(CreateKargoCompanyCommand command)
        {
            return GetActionResult(await _mediator.Send(command));
        }
        
        [HttpPut("{Id}")]
        public async Task<ActionResult<RequestResult<bool>>> Put(int Id, UpdateKargoCompanyCommand command)
        {
            command.Id = Id;
            return GetActionResult(await _mediator.Send(command));
        }
        
        [HttpDelete("{Id}")]
        public async Task<ActionResult<RequestResult<bool>>> Delete(int Id)
        {
            return GetActionResult(await _mediator.Send(new DeleteKargoCompanyCommand { Id = Id }));
        }
    }
}
