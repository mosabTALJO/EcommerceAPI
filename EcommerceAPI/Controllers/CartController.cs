using EcommerceAPI.Core.Carts.Command;
using EcommerceAPI.Core.Carts.Queries;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace EcommerceAPI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : MyControllerBase
    {
        //private readonly IStringLocalizer _stringLocalizer;
        public CartController(IMediator mediator) : base(mediator: mediator)
        {
            // _stringLocalizer = stringLocalizer;
        }

        [HttpGet("{CustomerID?}/{OrderId?}")]
        [Authorize(Policy = PoliciesText.UpdateOrder)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<OrderVM>>> Get(string CustomerID, int? OrderId = null)
        {
            return GetActionResult(await _mediator.Send(new GetCartByIdQuery { CustomerID = CustomerID, OrderId = OrderId }));
        }

        [HttpPost("CreateOrder")]
        [Authorize(Policy = PoliciesText.UpdateOrder)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> CreateOrder(CreateCratOrderCommand command)
        {
            command.UserId = User.Identity.Name;
            return GetActionResult(await _mediator.Send(command));
        }


        [HttpDelete("{CartID}/{CartItemID}")]
        [Authorize(Policy = PoliciesText.UpdateOrder)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> Delete(int CartID, int CartItemID)
        {
            return GetActionResult(await _mediator.Send(new DeleteCartItemCommand { CartId = CartID, CartItemId = CartItemID }));
        }

        //api/cart/CPConfirmOnDoorPaymentOrder/{OrderId}
        [HttpPost("CPConfirmOnDoorPaymentOrder/{OrderId}")]
        [Authorize(Policy = PoliciesText.UpdateOrder)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> ConfirmOnDoorPaymentOrder(int OrderId)
        {
            return GetActionResult(await _mediator.Send(new ConfirmOnDoorPaymentOrderCommand { OrderId = OrderId }));
        }

        // =========================================================================================== MOBILE
        [HttpGet("GetMyCart/{addressId?}")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<OrderVM>>> GetMyCart(int? addressId = null)
        {
            var containsCurrency = Request.Headers.ContainsKey("Currency");
            return await _mediator.Send(new GetMyCartQuery
            {
                AddressId = addressId,
                Currency = Request.Headers.TryGetValue("Currency", out StringValues s) ? s.ToString() : null,
                UserId = User.Identity?.Name
            });
        }

        [HttpPost("GetMyCartNoUser")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<OrderVM>>> GetMyCartNoUser([FromBody] CartItemVM[] items, [FromQuery] string? couponCode = null)
        {
            var containsCurrency = Request.Headers.ContainsKey("Currency");
            return GetActionResult(await _mediator.Send(new GetMyCartNoUserQuery
            {
                Items = items,
                CouponCode = couponCode,
                Currency = containsCurrency ? Request.Headers["Currency"].ToString() : null
            }));
        }

        [HttpGet("GetDeliveryData/{CountryCode}/{CityCode?}")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<DeliveryData>>> GetDeliveryData(string CountryCode, string CityCode)
        {
            return await _mediator.Send(new GetDeliveryDataQuery { CountryCode = CountryCode, CityCode = CityCode, UserId = User.Identity.Name });
        }

        [HttpPost("GetDeliveryDataNoUser/{CountryCode}/{CityCode}")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<DeliveryData>>> GetDeliveryDataNoUser([FromBody] CartItemVM[] items, string CountryCode, string CityCode)
        {
            return await _mediator.Send(new GetDeliveryDataNoUserQuery { Items = items, CountryCode = CountryCode, CityCode = CityCode });
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<bool>>> Post(CartItemVM[] items)
        {
            var containsCountryCode = Request.Headers.TryGetValue("Country-Code", out StringValues s);
            return GetActionResult(await _mediator.Send(new CreateCartItemsCommand
            {
                Items = items,
                UserId = User.Identity.Name,
                Currency = Request.Headers.TryGetValue("Currency", out StringValues c) ? c.ToString() : null,
                CountryCode = containsCountryCode ? s.ToString() : null,
                ContainsCountryCodeKey = containsCountryCode
            }));
        }

        [HttpPost("AddCoupon")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<bool>>> AddCoupon([FromQuery] string CouponCode)
        {
            return GetActionResult(await _mediator.Send(new AddCouponCommand
            {
                CouponCode = CouponCode,
                UserId = User.Identity.Name
            }));
        }

        [HttpDelete("RemoveCoupon")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<bool>>> RemoveCoupon()
        {
            return GetActionResult(await _mediator.Send(new RemoveCouponCommand { UserId = User.Identity.Name }));
        }

        [HttpPost("PaymentCancelled")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<bool>>> PaymentCancelled()
        {
            return GetActionResult(await _mediator.Send(new PaymentCancelledCommand { UserId = User.Identity.Name }));
        }

        [HttpGet("PayMyFatoorah/{UserAddressId}")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<string>>> PayMyFatoorah(int UserAddressId, [FromQuery] int? productNotFound = null, [FromQuery] string idCard = null)
        {
            return GetActionResult(await _mediator.Send(new PayMyFatoorahQuery
            {
                UserAddressId = UserAddressId,
                Currency = Request.Headers.TryGetValue("Currency", out StringValues c) ? c.ToString() : null,
                UserId = User.Identity?.Name,
                ProductNotFound = (ProductNotFound?)productNotFound,
                OrderDeviceType = Request.Headers.TryGetValue("device-type", out StringValues s) && int.TryParse(s, out int i) ? (OrderDeviceType?)i : null,
                IdCard = idCard?.Replace("\"", ""),
            }));
        }

        //api/cart/OnDoorPaymentPendingOrder/{userAddressId}
        [HttpPost("OnDoorPaymentPendingOrder/{userAddressId}")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<bool>>> OnDoorPaymentPendingOrder(int userAddressId, [FromQuery] int? productNotFound = null, [FromQuery] string idCard = null)
        {
            return GetActionResult(await _mediator.Send(new OnDoorPaymentPendingOrderCommand
            {
                UserAddressId = userAddressId,
                UserId = User.Identity?.Name,
                Currency = Request.Headers.TryGetValue("Currency", out StringValues c) ? c.ToString() : null,
                ProductNotFound = (ProductNotFound?)productNotFound,
                OrderDeviceType = Request.Headers.TryGetValue("device-type", out StringValues s) && int.TryParse(s, out int i) ? (OrderDeviceType?)i : null,
                IdCard = idCard?.Replace("\"", ""),
            }));
        }

        [HttpPost("ConfirmOnDoorPaymentOrder/{userAddressId}")] // for old versions
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<bool>>> ConfirmOnDoorPaymentOrderOld(int userAddressId, [FromQuery] int? productNotFound = null, [FromQuery] string idCard = null)
        {
            return GetActionResult(await _mediator.Send(new OnDoorPaymentPendingOrderCommand
            {
                UserAddressId = userAddressId,
                UserId = User.Identity.Name,
                Currency = Request.Headers.TryGetValue("Currency", out StringValues c) ? c.ToString() : null,
                ProductNotFound = (ProductNotFound?)productNotFound,
                OrderDeviceType = Request.Headers.TryGetValue("device-type", out StringValues s) && int.TryParse(s, out int i) ? (OrderDeviceType?)i : null,
                IdCard = idCard?.Replace("\"", ""),
            }));
        }

        [HttpPost("NoAccountPurchase")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<NoAccountPaymentVM>>> NoAccountPurchase([FromBody] NoAccountPurchaseCommand command)
        {
            command.Currency = Request.Headers.TryGetValue("Currency", out StringValues c) ? c.ToString() : null;
            command.OrderDeviceType = Request.Headers.TryGetValue("device-type", out StringValues s) && int.TryParse(s, out int i) ? (OrderDeviceType?)i : null;
            return GetActionResult(await _mediator.Send(command));
        }

        [HttpGet("GetOrderDetailsNoAccount/{OrderId}/{UserId}")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<OrderVM>>> GetOrderDetailsNoAccount(int OrderId, string UserId)
        {
            return GetActionResult(await _mediator.Send(new GetOrderDetailsNoAccountQuery { OrderId = OrderId, UserId = UserId }));
        }
    }
}