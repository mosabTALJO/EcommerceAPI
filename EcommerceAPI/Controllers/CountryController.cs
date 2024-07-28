using EcommerceAPI.Core.Countries.Commands;
using EcommerceAPI.Core.Countries.Queries;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceAPI.Web.Controllers
{
    public class CountryController : MyControllerBase
    {

        public CountryController(IMediator mediator) : base(mediator: mediator)
        {
        }

        //GetList api/Country
        [HttpGet]
        public async Task<ActionResult<RequestResult<CountryVM[]>>> Get()
        //[FromQuery] int pageNumber = 1,
        //[FromQuery] int pageSize = 20,
        //[FromQuery] string search = null,
        //[FromQuery] string sortField = "Id",
        //[FromQuery] string sortOrder = "Asc")
        {
            return GetActionResult(await _mediator.Send(new GetAllCountriesQuery
            {
                Role = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
            }));
        }

        //GetList api/Country
        [HttpGet("GetMainScreenCountries")]
        [Authorize(Policy = PoliciesText.EditCountries)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<List<CountryVM>>>> GetMainScreenCountries()
        {
            return GetActionResult(await _mediator.Send(new GetMainScreenCountriesQuery()));
        }

        // Post api/Country
        [HttpPost]
        [Authorize(Policy = PoliciesText.EditCountries)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> Post([FromBody] CreateCountryCommand command) =>
            GetActionResult(await _mediator.Send(command));

        // Put api/Country
        [HttpPut("{Id}")]
        [Authorize(Policy = PoliciesText.EditCountries)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> Put(int id, [FromBody] UpdateCountryCommand command)
        {
            command.Id = id;
            return GetActionResult(await _mediator.Send(command));
        }

        // Delete api/Country
        [HttpDelete("{Id}")]
        [Authorize(Policy = PoliciesText.EditCountries)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> Delete(int id)
        {
            var command = new DeleteCountryCommand
            {
                Id = id,
            };
            return GetActionResult(await _mediator.Send(command));
        }

        [HttpPut]
        [Authorize(Policy = PoliciesText.EditCountries)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> PutMult([FromBody] UpdateCountryCommand[] countries)
        {
            return GetActionResult(await _mediator.Send(new UpdateMultipleCountriesCommand { Countries = countries }));
        }
    }
}