using EcommerceAPI.Core.BaseItems.Command;
using EcommerceAPI.Core.BaseItems.Queries;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace EcommerceAPI.Web.Controllers
{
    public class BaseItemController : MyControllerBase
    {
        public BaseItemController(IMediator mediator) : base(mediator: mediator)
        {
        }

        // GETList
        [HttpGet]
        public async Task<ActionResult<RequestResult<ListResponse<BaseItemVM>>>> Get([FromQuery] int IsHomeOrCategory = 0)
        {
            var lang = CultureInfo.CurrentCulture?.TwoLetterISOLanguageName ?? "en";
            var currency = Request.Headers["Currency"].ToString();
            var role = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            var query = new GetBaseItemsQuery
            {
                IsHomeOrCategory = IsHomeOrCategory,
                Currency = currency,
                Role = role,
                UserId = User.Identity.Name,
                lang = lang,
            };
            return GetActionResult(await _mediator.Send(query));
        }

        // GETList
        [HttpGet("GetBaseItems")]
        public async Task<ActionResult<RequestResult<ListResponse<BaseItemVM>>>> GetBaseItems([FromQuery] int IsHomeOrCategory = 0)
        {
            var toCurrency = Request.Headers["Currency"].ToString();
            return GetActionResult(await _mediator.Send(new GetBaseItemsQuery
            {
                IsHomeOrCategory = IsHomeOrCategory,
                UserId = User.Identity.Name,
                ToCurrency = toCurrency,
                Role = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
            }));
        }

        // GETList
        [HttpGet("GetWebBaseItems")]
        public async Task<ActionResult<RequestResult<ListResponse<BaseItemVM>>>> GetWebBaseItems([FromQuery] int IsHomeOrCategory = 0)
        {
            var toCurrency = Request.Headers["Currency"].ToString();
            return GetActionResult(await _mediator.Send(new GetWebBaseItemsQuery
            {
                IsHomeOrCategory = IsHomeOrCategory,
                ToCurrency = toCurrency,
                UserId = User.Identity.Name,
                Role = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
            }));
        }

        [HttpPost]
        [Authorize(Policy = PoliciesText.EditBaseItem)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<int>>> Post([FromBody] CreateBaseItemCommand command)
        {
            return GetActionResult(await _mediator.Send(command));
        }

        [HttpPut("{Id}")]
        [Authorize(Policy = PoliciesText.EditBaseItem)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> PutItem(int Id, [FromBody] UpdateBaseItemCommand command)
        {
            command.Id = Id;
            return GetActionResult(await _mediator.Send(command));
        }

        [HttpPut("OrderItems")]
        [Authorize(Policy = PoliciesText.EditBaseItem)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> Put([FromBody] BaseItemVM[] items)
        {
            return GetActionResult(await _mediator.Send(new OrderBaseItemsCommand { Items = items }));
        }

        [HttpDelete("{Id}")]
        [Authorize(Policy = PoliciesText.EditBaseItem)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> Delete(int Id)
        {
            return GetActionResult(await _mediator.Send(new DeleteBaseItemCommand { Id = Id }));
        }
    }
}