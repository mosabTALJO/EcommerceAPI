using EcommerceAPI.Core.EmailPromotions.Commands;
using EcommerceAPI.Core.EmailPromotions.Queries;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EcommerceAPI.Web.Controllers
{
    public class EmailPromotionController : MyControllerBase
    {
        public EmailPromotionController(IMediator mediator) : base(mediator: mediator)
        {
        }

        [HttpGet]
        [Authorize(Policy = PoliciesText.EmailPromotion)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<ListResponse<EmailPromotionLiteVM>>>> Get(
            [FromQuery] string subject = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            return GetActionResult(await _mediator.Send(new GetAllEmailPromotionsQuery { Subject = subject, PageNumber = pageNumber, PageSize = pageSize }));
        }

        [HttpGet("{Id}")]
        [Authorize(Policy = PoliciesText.EmailPromotion)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<EmailPromotionVM>>> Get(int Id)
        {
            return GetActionResult(await _mediator.Send(new GetEmailPromotionByIdQuery { Id = Id }));
        }


        [HttpPost]
        [Authorize(Policy = PoliciesText.EmailPromotion)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<int>>> Post(CreateEmailPromotionCommand command)
        {
            if (Debugger.IsAttached)
                return BadRequest("This feature is working only on live");
            return GetActionResult(await _mediator.Send(command));
        }

        [HttpPost("SendTestEmail")]
        [Authorize(Policy = PoliciesText.EmailPromotion)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<int>>> SendTestEmail([FromBody] CreateEmailPromotionCommand command)
        {
            if (string.IsNullOrEmpty(command.TestEmail))
                command.TestEmail = "info@trgatestore.com";

            return GetActionResult(await _mediator.Send(command));
        }
    }
}
