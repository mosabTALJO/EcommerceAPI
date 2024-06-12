using EcommerceAPI.Core.UserAddresses.Commands;
using EcommerceAPI.Core.UserAddresses.Queries;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    public class UserAddressController : MyControllerBase
    {
        public UserAddressController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("GetUserAddresses/{CustomerId}")]
        [Authorize(Policy = PoliciesText.EditCustomers)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<UserAddressVM[]>>> Get(string CustomerId)
        {
            return GetActionResult(await _mediator.Send(new GetUserAddressesQuery { UserId = CustomerId }));
        }
        [HttpPost("CreateCustomerAddress/{CustomerId}")]
        [Authorize(Policy = PoliciesText.EditCustomers)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<bool>> CreateCustomerAddress(string CustomerId, [FromBody] CreateUserAddressCommand command)
        {
            command.UserId = CustomerId;
            var x = await _mediator.Send(command);
            return x.ActionResultType == ActionResultType.OK ? Ok(true) : BadRequest(x.ErrorText);
        }
        [HttpPut("UpdateCustomerAddress/{Id}/{CustomerId}")]
        [Authorize(Policy = PoliciesText.EditCustomers)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> UpdateCustomerAddress(int Id, string CustomerId, [FromBody] UpdateUserAddressCommand command)
        {
            command.Id = Id;
            command.UserId = CustomerId;
            return GetActionResult(await _mediator.Send(command));
        }
        [HttpDelete("{Id}/{CustomerId}")]
        [Authorize(Policy = PoliciesText.EditCustomers)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> Delete(int Id, string CustomerId)
        {
            return GetActionResult(await _mediator.Send(new DeleteUserAddressCommand
            {
                UserId = CustomerId,
                Id = Id
            }));
        }

        //// Get api/UserAddress
        [HttpGet()]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<UserAddressVM[]>>> Get()
        {
            return GetActionResult(await _mediator.Send(new GetUserAddressesQuery { UserId = User.Identity.Name }));
        }
        //// Post api/UserAddress
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<bool>> Post([FromBody] CreateUserAddressCommand command)
        {
            command.UserId = User.Identity.Name;
            var x = await _mediator.Send(command);
            return x.ActionResultType == ActionResultType.OK ? Ok(true) : BadRequest(x.ErrorText);
        }
        //// Post api/UserAddress
        [HttpPost("CreateAddress")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<int>>> Post2([FromBody] CreateUserAddressCommand command)
        {
            command.UserId = User.Identity.Name;
            return GetActionResult(await _mediator.Send(command));
        }

        // Put api/UserAddress
        [HttpPut("{Id}")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<bool>>> Put(int id, [FromBody] UpdateUserAddressCommand command)
        {
            command.Id = id;
            command.UserId = User.Identity.Name;
            return GetActionResult(await _mediator.Send(command));
        }

        // Delete api/UserAddress
        [HttpDelete("{Id}")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<bool>>> Delete(int Id)
        {
            return GetActionResult(await _mediator.Send(new DeleteUserAddressCommand
            {
                UserId = User.Identity.Name,
                Id = Id
            }));
        }

        [HttpGet("FindAddress/{lat}/{lon}")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<UserAddressVM>>> FindAddress(double lat, double lon)
        {
            return GetActionResult(await _mediator.Send(new FindUserAddressQuery
            {
                UserId = User.Identity.Name,
                Lat = lat,
                Lon = lon,
            }));
        }
    }
}
