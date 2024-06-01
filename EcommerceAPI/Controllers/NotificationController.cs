using EcommerceAPI.Core.Notifications.Commands;
using EcommerceAPI.Core.Notifications.Queries;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EcommerceAPI.Web.Controllers
{
    public class NotificationController : MyControllerBase
    {
        public NotificationController(IMediator mediator) : base(mediator: mediator)
        {
        }

        // GETList api/Notification
        [HttpGet]
        [ApiExplorerSettings(GroupName = "fe")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<RequestResult<ListResponse<NotificationVM>>>> Get(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string sortField = "Id",
            [FromQuery] string sortOrder = "Asc")
        {
            return GetActionResult(await _mediator.Send(new GetAllUserIncludedNotificationsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortField = sortField,
                SortOrder = sortOrder,
                UserId = User.Identity.Name
            }));
        }


        // GETList api/Notification
        [HttpGet("GetAll")]
        [ApiExplorerSettings(GroupName = "cp")]
        [Authorize(Policy = PoliciesText.EditNotifications)]
        public async Task<ActionResult<RequestResult<ListResponse<NotificationVM>>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string sortField = "Id",
            [FromQuery] string sortOrder = "Asc")
        {
            return GetActionResult(await _mediator.Send(new GetAllNotificationsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortField = sortField,
                SortOrder = sortOrder,
            }));
        }

        [HttpPost]
        [ApiExplorerSettings(GroupName = "cp")]
        [Authorize(Policy = PoliciesText.EditNotifications)]
        public async Task<ActionResult<RequestResult<int>>> Post([FromBody] CreateNotificationCommand command)
        {
            if (Debugger.IsAttached)
                return Ok(true);
            return GetActionResult(await _mediator.Send(command));
        }

        [HttpPost("SendNotification")]
        [ApiExplorerSettings(GroupName = "cp")]
        [Authorize(Policy = PoliciesText.EditNotifications)]
        public async Task<ActionResult<RequestResult<int>>> SendNotification([FromBody] SendNotificationCommand command)
        {
            if (Debugger.IsAttached)
                return Ok(true);
            return GetActionResult(await _mediator.Send(command));
        }
    }
}
