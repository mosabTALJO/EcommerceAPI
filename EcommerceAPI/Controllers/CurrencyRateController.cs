using EcommerceAPI.Core.CurrencyRates.Commands;
using EcommerceAPI.Core.CurrencyRates.Queries;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    [Authorize(Policy = PoliciesText.EditCurrencyRate)]
    public class CurrencyRateController : MyControllerBase
    {
        public CurrencyRateController(IMediator mediator) : base(mediator: mediator)
        {
        }

        // GETList api/CurrencyRate
        [HttpGet]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<CurrencyRateVM[]>>> Get() =>
            GetActionResult(await _mediator.Send(new GetAllCurrencyRatesQuery()));

        [HttpPut]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> Put([FromBody] CurrencyRateVM[] items)
        {
            return GetActionResult(await _mediator.Send(new UpdateCurrencyRateCommand(items)));
        }

        [HttpPut("Refresh")]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> RefreshCurrencies()
        {
            return GetActionResult(await _mediator.Send(new RefreshCurrenciesCommand()));
        }
    }
}
