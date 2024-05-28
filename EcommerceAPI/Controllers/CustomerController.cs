using EcommerceAPI.Core.Customers.Commands;
using EcommerceAPI.Core.Customers.Queries;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    [ApiExplorerSettings(GroupName = "cp")]
    [Authorize(Policy = PoliciesText.EditCustomers)]
    public class CustomerController : MyControllerBase
    {
        public CustomerController(IMediator mediator) : base(mediator: mediator)
        {
        }

        // GET api/Customer
        [HttpGet]
        public async Task<ActionResult<RequestResult<ListResponse<UserVM>>>> Get(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string search = "",
            [FromQuery] string sortField = "CreatedAt",
            [FromQuery] string sortOrder = "Asc")
        {
            return GetActionResult(await _mediator.Send(new GetAllCustomersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = search,
                SortField = sortField,
                SortOrder = sortOrder
            }));
        }

        [HttpGet("ExportCustomers")]
        public async Task<ActionResult<RequestResult<ExportCustomerVM[]>>> ExportCustomers(
            [FromQuery] int CountryId)
        {
            return GetActionResult(await _mediator.Send(new ExportCustomersQuery { CountryId = CountryId }));
        }

        // GET api/Customer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RequestResult<UserVM>>> Get(string id)
        {
            return GetActionResult(await _mediator.Send(new GetCustomerByIdQuery { Id = id }));
        }

        // PUT api/Customer/5
        [HttpPut("{id}")]
        public async Task<ActionResult<RequestResult<bool>>> UpdateCustomer(string id, [FromBody] UpdateCustomerCommand command)
        {
            command.Id = id;
            return GetActionResult(await _mediator.Send(command));
        }

        // Delete api/customer/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RequestResult<bool>>> DeleteCustomer(string id)
        {
            return GetActionResult(await _mediator.Send(new DeleteCustomerCommand { Id = id }));
        }
    }
}