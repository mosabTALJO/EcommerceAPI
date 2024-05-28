using EcommerceAPI.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace EcommerceAPI.Web.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    [ApiController]
    public class MyControllerBase : ControllerBase
    {
        public readonly IMediator _mediator;

        public MyControllerBase(IMediator mediator = null)
        {
            _mediator = mediator;
        }

        //public ActionResult<RequestResult<T>> GetActionResult<T>(T data, string? error = null, ActionResultType actionResultType = ActionResultType.OK)
        //{
        //    return actionResultType switch
        //    {
        //        ActionResultType.OK => Ok(new RequestResult<T>(data)),
        //        ActionResultType.NotFound => NotFound(new RequestResult<T>(error)),
        //        ActionResultType.Unauthorized => Unauthorized(new RequestResult<T>(error)),
        //        _ => BadRequest(new RequestResult<T>(error)),
        //    };
        //}
        public ActionResult<RequestResult<T>> GetActionResult<T>(RequestResult<T> requestResult)
        {
            return requestResult.ActionResultType switch
            {
                ActionResultType.OK => Ok(requestResult),
                ActionResultType.NotFound => NotFound(requestResult),
                ActionResultType.Unauthorized => Unauthorized(requestResult),
                _ => BadRequest(requestResult),
            };
        }
    }
}