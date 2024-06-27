using EcommerceAPI.Core.Coupons.Commands;
using EcommerceAPI.Core.Coupons.Queries;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    public class CouponController : MyControllerBase
    {
        public CouponController(IMediator mediator) : base(mediator: mediator)
        {
        }

        // GETList api/Coupon
        [HttpGet]
        [Authorize(Policy = PoliciesText.EditCoupons)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<ListResponse<CouponVM>>>> Get(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string search = null,
            [FromQuery] string sortField = "Id",
            [FromQuery] string sortOrder = "Asc")
        {
            return GetActionResult(await _mediator.Send(new GetAllCouponsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = search,
                SortField = sortField,
                SortOrder = sortOrder
            }));
        }

        [HttpGet("{Id}")]
        [Authorize(Policy = PoliciesText.EditCoupons)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<CouponVM>>> GetCouponById(int Id)
        {
            return GetActionResult(await _mediator.Send(new GetCouponByIdQuery { CouponId = Id }));
        }

        [HttpGet("UserCoupons")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<CouponVM[]>>> GetUserCoupons()
        {
            return GetActionResult(await _mediator.Send(new GetUserCouponsQuery { UserId = User.Identity.Name }));
        }

        [HttpPost]
        [Authorize(Policy = PoliciesText.EditCoupons)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<int>>> Post([FromBody] CreateCouponCommand command)
        {
            return GetActionResult(await _mediator.Send(command));
        }

        // Put api/Coupon
        [HttpPut("{Id}")]
        [Authorize(Policy = PoliciesText.EditCoupons)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> Put(int id, [FromBody] UpdateCouponCommand command)
        {
            command.Id = id;
            return GetActionResult(await _mediator.Send(command));
        }

        // Delete api/Coupon
        [HttpDelete("{Id}")]
        [Authorize(Policy = PoliciesText.EditCoupons)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> Delete(int Id)
        {
            return GetActionResult(await _mediator.Send(new DeleteCouponCommand { Id = Id }));
        }
    }
}
