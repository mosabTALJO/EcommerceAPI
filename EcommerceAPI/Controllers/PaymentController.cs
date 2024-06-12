using EcommerceAPI.Core.Payments.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    [ApiExplorerSettings(GroupName = "cpX")]
    public class PaymentController : MyControllerBase
    {
        public PaymentController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("myfatoorah/success")]
        public async Task MyfatoorahSuccess([FromQuery] string paymentId)
        {
            await _mediator.Send(new MyfatoorahSuccessQuery { PaymentId = paymentId });
        }
        [HttpGet("myfatoorah/failed")]
        public async Task MyfatoorahFailed([FromQuery] string paymentId)
        {
            await _mediator.Send(new MyfatoorahFailedQuery { PaymentId = paymentId });
        }
    }
}