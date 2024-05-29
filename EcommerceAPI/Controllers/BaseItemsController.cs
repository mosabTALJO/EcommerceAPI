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
        private readonly ICurrencyRateService _currencyRateService;
        public BaseItemController(IMediator mediator, ICurrencyRateService currencyRateService) : base(mediator: mediator)
        {
            _currencyRateService = currencyRateService;
        }

        // GETList
        [HttpGet]
        public async Task<ActionResult<RequestResult<ListResponse<BaseItemVM>>>> Get([FromQuery] int IsHomeOrCategory = 0)
        {
            var lang = CultureInfo.CurrentCulture?.TwoLetterISOLanguageName ?? "en";
            var currencyRate = _currencyRateService.GetCurrencyRate(Request.Headers["Currency"]);
            string role = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            var query = new GetBaseItemsQuery
            {
                CurrencyRate = currencyRate,
                IsHomeOrCategory = IsHomeOrCategory,
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
            return GetActionResult(await _mediator.Send(new GetBaseItemsQuery
            {
                IsHomeOrCategory = IsHomeOrCategory,
                UserId = User.Identity.Name,
                Role = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
                CurrencyRate = _currencyRateService.GetCurrencyRate(Request.Headers["Currency"])
            }));
        }

        // GETList
        [HttpGet("GetWebBaseItems")]
        public async Task<ActionResult<RequestResult<ListResponse<BaseItemVM>>>> GetWebBaseItems([FromQuery] int IsHomeOrCategory = 0)
        {
            return GetActionResult(await _mediator.Send(new GetWebBaseItemsQuery
            {
                IsHomeOrCategory = IsHomeOrCategory,
                UserId = User.Identity.Name,
                Role = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
                CurrencyRate = _currencyRateService.GetCurrencyRate(Request.Headers["Currency"])
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