using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.Core.Orderings.Queries;
using EcommerceAPI.Core.Orderings.Commands;

namespace EcommerceAPI.Web.Controllers
{
    public class OrderingController : MyControllerBase
    {

        public OrderingController(IMediator mediator) : base(mediator: mediator)
        {
        }

        [HttpGet]
        [Authorize(Policy = PoliciesText.UpdateProduct)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<OrderCategoryBrandVM[]>>> Get([FromQuery] int categoryId, [FromQuery] string search = null)
        {
            return GetActionResult(await _mediator.Send(new GetOrderingQuery { CategoryId = categoryId, Search = search }));
        }

        [HttpPost]
        [Authorize(Policy = PoliciesText.UpdateProduct)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> Post(OrderCategoryBrand[] items)
        {
            return GetActionResult(await _mediator.Send(new CreateOrderingCommand { Items = items }));
        }
    }
}