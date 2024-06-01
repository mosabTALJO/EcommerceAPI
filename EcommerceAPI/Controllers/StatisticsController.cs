using EcommerceAPI.Core.Statistics;
using EcommerceAPI.Core.Statistics.Queries;
using EcommerceAPI.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    public class StatisticsController : MyControllerBase
    {
        public StatisticsController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("OrdersData")]
        [Authorize(Policy = PoliciesText.EditAdmins)]
        public async Task<ActionResult<RequestResult<Chart>>> OrdersData()
        {
            return GetActionResult(await _mediator.Send(new GetOrdersDataQuery()));
        }

        [HttpGet("RevenueData")]
        [Authorize(Policy = PoliciesText.EditAdmins)]
        public async Task<ActionResult<RequestResult<RevenueChart>>> RevenueData([FromQuery] int CountryId = 0, [FromQuery] DateTime? From = null, [FromQuery] DateTime? To = null)
        {
            return GetActionResult(await _mediator.Send(new GetRevenueDataQuery() { CountryId = CountryId, From = From, To = To }));
        }

        [HttpGet("RevenueDataTable")]
        [Authorize(Policy = PoliciesText.EditAdmins)]
        public async Task<ActionResult<RequestResult<RevenueDataTable[]>>> RevenueDataTable([FromQuery] DateTime? From = null, [FromQuery] DateTime? To = null)
        {
            return GetActionResult(await _mediator.Send(new GetRevenueDataTableQuery() { From = From, To = To }));
        }

        [HttpGet("NewCustomersCount")]
        [Authorize(Policy = PoliciesText.EditAdmins)]
        public async Task<ActionResult<RequestResult<DayWeekMonth>>> NewCustomersCount()
        {
            return GetActionResult(await _mediator.Send(new GetNewCustomersCountQuery()));
        }
    }
}