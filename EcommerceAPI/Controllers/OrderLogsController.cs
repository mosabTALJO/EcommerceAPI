using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.Core.OrderLogs.Queries;

namespace EcommerceAPI.Web.Controllers
{
    [ApiExplorerSettings(GroupName = "cp")]
    [Authorize(Policy = PoliciesText.UpdateOrder_GetLogs)]
    public class OrderLogsController : MyControllerBase
    {
        public OrderLogsController(IMediator mediator) : base(mediator: mediator)
        {
        }

        [HttpGet("Order/{OrderId}")]
        public async Task<ActionResult<OrderLogVM[]>> Get(int OrderId)
        {
            return Ok(await _mediator.Send(new GetOrderLogsQuery { OrderId = OrderId }));
        }
        
        [HttpGet("Item/{OrderItemId}")]
        public async Task<ActionResult<OrderLogVM[]>> GetOrderItemLogs(int OrderItemId)
        {
            return Ok(await _mediator.Send(new GetOrderItemLogsQuery { OrderItemId = OrderItemId }));
        }
    }
}
