using EcommerceAPI.Core.Ratings.Commands;
using EcommerceAPI.Core.Ratings.Queries;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    public class RatingController : MyControllerBase
    {
        //private readonly IStringLocalizer _stringLocalizer;
        public RatingController(IMediator mediator) : base(mediator: mediator)
        {
            //_stringLocalizer = stringLocalizer;
        }

        [HttpGet("GetRatings")]
        [Authorize(Policy = PoliciesText.EditRatings)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<ListResponse<OrderRatingVM>>>> GetRatings(
            [FromQuery] int pageNumber = 0,
            [FromQuery] int pageSize = 20,
            [FromQuery] string sortField = "CreatedAt",
            [FromQuery] string sortOrder = "Asc",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            return GetActionResult(await _mediator.Send(new GetRatingsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortField = sortField,
                SortOrder = sortOrder,
                StartDate = startDate,
                EndDate = endDate
            }));
        }

        [HttpGet("OrderRatings/{OrderId}")]
        [Authorize(Policy = PoliciesText.EditRatings)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<List<RatingVM>>>> GetOrderRating(int OrderId)
        {
            return GetActionResult(await _mediator.Send(new GetOrderRatingByOrderIdQuery { OrderId = OrderId }));
        }

        [HttpPost("RateOrder")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<bool>>> RateOrder([FromBody] CreateOrderRatingCommand command)
        {
            command.UserId = User.Identity.Name;
            return GetActionResult(await _mediator.Send(command));
        }
    }
}
