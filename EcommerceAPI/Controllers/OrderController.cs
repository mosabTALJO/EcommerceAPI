using EcommerceAPI.Core.Orders.Commands;
using EcommerceAPI.Core.Orders.Queries;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    public class OrderController : MyControllerBase
    {
        //private readonly IStringLocalizer _stringLocalizer;
        public OrderController(IMediator mediator) : base(mediator: mediator)
        {
            //_stringLocalizer = stringLocalizer;
        }

        // GETList api/Order
        [HttpGet]
        [Authorize(Policy = PoliciesText.GetOrders)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<ListResponse<OrderLiteVM>>>> Get(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string customerId = null,
            [FromQuery] string search = null,
            [FromQuery] string Country = null,
            [FromQuery] string City = null,
            [FromQuery] string sortField = "Id",
            [FromQuery] string sortOrder = "Asc",
            [FromQuery] string orderStatus = null,
            [FromQuery] bool showArchived = false)
        {
            return GetActionResult(await _mediator.Send(new GetAllOrdersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                customerId = customerId,
                Search = search,
                Country = Country,
                City = City,
                SortField = sortField,
                SortOrder = sortOrder,
                OrderStatus = orderStatus,
                ShowArchived = showArchived
            }));
        }

        // GET GetMyOrders api/Order
        [HttpGet("GetMyOrders")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<ListResponse<OrderLiteVM>>>> GetMyOrders(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            return GetActionResult(await _mediator.Send(new GetMyOrdersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                UserId = User.Identity.Name
            }));
        }

        // GET GetMyOrderDetails api/Order/GetMyOrderDetails/{OrderId}
        [HttpGet("GetMyOrderDetails/{OrderId}")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<OrderVM>>> GetMyOrderDetails(int OrderId)
        {
            return GetActionResult(await _mediator.Send(new GetMyOrderDetailsQuery
            {
                OrderId = OrderId,
                UserId = User.Identity.Name
            }));
        }

        // GET GetInvoiceDetails api/Order/GetInvoiceDetails/{OrderId}
        [HttpGet("GetInvoiceDetails/{OrderId}")]
        [Authorize(Policy = PoliciesText.UpdateOrder_ShowInvoice)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<PaymentDataModel>>> GetInvoiceDetails(int OrderId)
        {
            return GetActionResult(await _mediator.Send(new GetInvoiceDetailsQuery
            {
                OrderId = OrderId,
            }));
        }

        // GET GetInvoiceDetails api/Order/GetInvoiceDetails/{OrderId}
        [HttpPost("CaptureInvoice/{OrderId}")]
        [Authorize(Policy = PoliciesText.UpdateOrder_CaptureInvoice)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> CaptureInvoice(int OrderId)
        {
            var command = new CaptureInvoiceCommand
            {
                OrderId = OrderId,
            };
            return GetActionResult(await _mediator.Send(command));
        }

        // GET api/orders/{id}
        [HttpGet("{id}")]
        [Authorize(Policy = PoliciesText.GetOrders)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<OrderVM>>> Get(int id)
        {
            return GetActionResult(await _mediator.Send(new GetOrderByIdQuery { Id = id }));
        }

        [HttpPost("AcceptItem/{OrderId}/{OrderItemID}")]
        [Authorize(Policy = PoliciesText.UpdateOrder)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> AcceptItem(int OrderId, int OrderItemID)
        {
            var command = new AcceptItemCommand()
            {
                OrderItemId = OrderItemID,
                OrderId = OrderId,
                UserId = User.Identity.Name,
            };
            return GetActionResult(await _mediator.Send(command));
        }

        [HttpPost("InStockItem/{OrderId}/{OrderItemID}")]
        [Authorize(Policy = PoliciesText.UpdateOrder)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> InStockItem(int OrderId, int OrderItemID)
        {
            return GetActionResult(await _mediator.Send(new InStockItemCommand { OrderId = OrderId, OrderItemId = OrderItemID, UserId = User.Identity.Name }));
        }

        [HttpPut("UpdateAcceptedItem/{OrderId}/{OrderItemID}")]
        [Authorize(Policy = PoliciesText.UpdateOrder)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> UpdateAcceptedItem([FromBody] UpdateAcceptedItemCommand command, int OrderId, int OrderItemID)
        {
            command.OrderItemId = OrderItemID;
            command.OrderId = OrderId;
            command.UserId = User.Identity.Name;
            return GetActionResult(await _mediator.Send(command));
        }

        [HttpPost("EditOrderItemData")]
        [Authorize(Policy = PoliciesText.UpdateOrder)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> EditOrderItemData(EditOrderItemDataCommand command)
        {
            command.UserId = User.Identity.Name;
            return GetActionResult(await _mediator.Send(command));
        }

        [HttpPost("RejectItem/{OrderId}/All")]
        [Authorize(Policy = PoliciesText.UpdateOrder)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> RejectAllItems(int OrderId)
        {
            return GetActionResult(await _mediator.Send(new RejectAllItemsCommand { OrderId = OrderId, UserId = User.Identity.Name }));
        }

        [HttpPost("RejectItem/{OrderId}/{OrderItemID}")]
        [Authorize(Policy = PoliciesText.UpdateOrder)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> RejectItem(int OrderId, int OrderItemID)
        {
            return GetActionResult(await _mediator.Send(new RejectItemByIdCommand { OrderId = OrderId, OrderItemID = OrderItemID, UserId = User.Identity.Name }));
        }

        [HttpPost("CancelItem/{OrderId}/{OrderItemID}")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<bool>>> CancelItem(int OrderId, int OrderItemID)
        {
            return GetActionResult(await _mediator.Send(new CancelItemCommand { OrderId = OrderId, OrderItemID = OrderItemID, UserId = User.Identity.Name }));
        }

        [HttpPost("CancelItem/{OrderId}/All")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<bool>>> CancelAllItems(int OrderId)
        {
            return GetActionResult(await _mediator.Send(new CancelAllItemsCommand { OrderId = OrderId, UserId = User.Identity.Name }));
        }

        [HttpPost("Return/{OrderId}/{OrderItemID}")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<bool>>> ReturnItem(int OrderId, int OrderItemID, [FromQuery] string returnReason)
        {
            return GetActionResult(await _mediator.Send(new ReturnItemCommand
            {
                OrderId = OrderId,
                OrderItemID = OrderItemID,
                UserId = User.Identity.Name,
                ReturnReason = returnReason
            }));
        }

        // Delete api/Order
        [HttpDelete("{Id}")]
        [Authorize(Policy = PoliciesText.DeleteOrder)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> Delete(int Id)
        {
            return GetActionResult(await _mediator.Send(new DeleteOrderCommand { Id = Id }));
        }


        // Delete api/Order
        [HttpPost("GeneratePaymentLink/{OrderId}")]
        [Authorize(Policy = PoliciesText.DeleteOrder)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<IActionResult> GeneratePaymentLink(int OrderId)
        {
            return Ok("https://Ecommerce.usos.co");
        }

        // Archive api/Order
        [HttpPut("Archive/{Id}")]
        [Authorize(Policy = PoliciesText.UpdateOrder)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> Archive(int Id)
        {
            return GetActionResult(await _mediator.Send(new ArchiveOrderCommand { Id = Id, UserId = User.Identity.Name }));
        }

        // Archive api/Order
        [HttpPut("UnArchive/{Id}")]
        [Authorize(Policy = PoliciesText.UpdateOrder)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> UnArchive(int Id)
        {
            return GetActionResult(await _mediator.Send(new UnArchiveOrderCommand { Id = Id, UserId = User.Identity.Name }));
        }

        [HttpGet("GetMergeableOrders/{Id}")]
        [Authorize(Policy = PoliciesText.GetOrders)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<List<int>>>> GetMergeableOrders(int Id)
        {
            return GetActionResult(await _mediator.Send(new GetMergeableOrdersQuery { Id = Id }));
        }

        [HttpPost("MergeOrders/{FirstId}/{SecondId}")]
        [Authorize(Policy = PoliciesText.UpdateOrder)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<int>>> MergeOrders(int FirstId, int SecondId)
        {
            return GetActionResult(await _mediator.Send(new MergeOrdersCommand { FirstId = FirstId, SecondId = SecondId, UserId = User.Identity.Name }));
        }

        [HttpGet("GetActive")]
        [Authorize(Policy = PoliciesText.GetOrders)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<List<OrderVM>>>> GetActiveOrders(
            [FromQuery] string counrty = null,
            [FromQuery] string city = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string search = null)
        {
            return GetActionResult(await _mediator.Send(new GetActiveOrdersQuery
            {
                City = city,
                Country = counrty,
                StartDate = startDate,
                EndDate = endDate,
                Search = search
            }));
        }

        [HttpPut("TogglePaymentType/{OrderId}")]
        [Authorize(Policy = PoliciesText.UpdateOrder)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> TogglePaymentType(int OrderId)
        {
            return GetActionResult(await _mediator.Send(new TogglePaymentTypeCommand { OrderId = OrderId, UserId = User.Identity.Name }));
        }

        [HttpPut("ChangeDeliveryPrice/{OrderID}/{NewDeliveryPrice}")]
        [Authorize(Policy = PoliciesText.UpdateOrder)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> ChangeDeliveryPrice(int OrderId, double NewDeliveryPrice)
        {
            return GetActionResult(await _mediator.Send(new ChangeDeliveryPriceCommand
            {
                UserId = User.Identity.Name,
                OrderId = OrderId,
                NewDeliveryPrice = NewDeliveryPrice
            }));
        }

        [HttpPut("EditRecipientInfo/{OrderID}")]
        [Authorize(Policy = PoliciesText.UpdateOrder_EditRecipientInfo)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> EditRecipientInfo(int OrderId, [FromBody] EditRecipientInfoCommand command)
        {
            command.OrderId = OrderId;
            command.UserId = User.Identity.Name;
            return GetActionResult(await _mediator.Send(command));
        }

        [HttpPut("AddSpecialDiscount/{OrderID}")]
        [Authorize(Policy = PoliciesText.UpdateOrder_SpecialDiscount)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> AddSpecialDiscount(int OrderId, [FromQuery] double specialDiscountAmount)
        {
            return GetActionResult(await _mediator.Send(new AddSpecialDiscountCommand
            {
                OrderId = OrderId,
                SpecialDiscountAmount = specialDiscountAmount,
                UserId = User.Identity.Name
            }));
        }
    }
    enum GetOrderStatus
    {
        All,
        Pending,
        Accepted,
        Rejected,
        Shipped,
        Delivered,
        Paid,
        ConfirmedForOnDoorPayment,
        Completed,
        Cart,
    }
}
