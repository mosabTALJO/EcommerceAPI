using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EcommerceAPI.Core.Inits.Queries;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;

namespace EcommerceAPI.Web.Controllers
{
    [ApiExplorerSettings(GroupName = "fe")]
    // [Authorize(Roles = "Customer")]
    public class InitController : MyControllerBase
    {
        private readonly ICurrencyRateService _currencyRateService;
        public InitController(IMediator mediator, ICurrencyRateService currencyRateService) : base(mediator: mediator)
        {
            _currencyRateService = currencyRateService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<RequestResult<InitDataModel>>> GetInitData([FromQuery] bool showHidden = false)
        {
            return GetActionResult(await _mediator.Send(new GetInitDataQuery 
            {
                UserId = User.Identity.Name,
                ShowHidden = showHidden
            }));
        }

        [HttpGet("GetInit")]
        [AllowAnonymous]
        public async Task<ActionResult<RequestResult<InitData>>> GetInit()
        {
            return GetActionResult(await _mediator.Send(new GetInitQuery 
            {
                Role = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
                Currency = Request.Headers["Currency"],
                UserId = User.Identity.Name
            }));
        }

        [HttpGet("WebInit")]
        [AllowAnonymous]
        public async Task<ActionResult<RequestResult<WebInitData>>> GetWebInit()
        {
            return GetActionResult(await _mediator.Send(new GetWebInitQuery 
            {
                Role = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
                Currency = Request.Headers["Currency"],
                CurrencyRate = _currencyRateService.GetCurrencyRate(Request.Headers["Currency"]),
                UserId = User.Identity.Name
            }));
        }
    }
}