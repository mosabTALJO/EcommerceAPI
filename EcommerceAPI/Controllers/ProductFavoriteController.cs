using EcommerceAPI.Core.ProductFavorites.Commands;
using EcommerceAPI.Core.ProductFavorites.Queries;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    public class ProductFavoriteController : MyControllerBase
    {
        private readonly ICurrencyRateService _currencyRateService;
        public ProductFavoriteController(IMediator mediator, ICurrencyRateService currencyRateService) : base(mediator)
        {
            _currencyRateService = currencyRateService;
        }

        // GETList api/ProductFavorite
        [HttpGet]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<ProductLiteVM[]>>> Get()
        {
            return GetActionResult(await _mediator.Send(new GetFavoriteProductsQuery
            {
                CurrencyRate = _currencyRateService.GetCurrencyRate(Request.Headers["Currency"]),
                UserId = User.Identity.Name
            }));
        }

        // GETList api/ProductFavorite
        [HttpGet("GetCustomerFavorites/{CustomerID}")]
        [Authorize(Policy = PoliciesText.EditCustomers)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<ProductLiteVM[]>>> GetCustomerFavorites(string CustomerID)
        {
            return GetActionResult(await _mediator.Send(new GetFavoriteProductsQuery
            {
                CurrencyRate = _currencyRateService.GetCurrencyRate("kwd"),
                UserId = CustomerID,
            }));
        }

        [HttpPost("{ProductID}")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<bool>>> Post(int ProductID)
        {
            return GetActionResult(await _mediator.Send(new CreateFavoriteProductCommand { ProductId = ProductID, UserId = User.Identity.Name }));
        }

        // Delete api/ProductFavorite
        [HttpDelete("{ProductID}")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<bool>>> Delete(int ProductID)
        {
            return GetActionResult(await _mediator.Send(new DeleteFavoriteProductCommand { ProductID = ProductID }));
        }
    }
}
