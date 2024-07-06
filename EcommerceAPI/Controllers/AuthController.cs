using EcommerceAPI.Core.Auths.Commands;
using EcommerceAPI.Core.Auths.Queries;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    public class AuthController : MyControllerBase
    {
        public AuthController(IMediator mediator) : base(mediator)
        {
        }

        [Route("/api/login")] // /login
        [HttpPost]
        public async Task<ActionResult<RequestResult<TokenViewModel>>> Login([FromBody] LoginCommand command)
        {
            return GetActionResult(await _mediator.Send(command));
        }

        [Route("/api/Register")]
        [HttpPost]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<TokenViewModel>>> Register([FromBody] RegisterUserCommand command)
        {
            return GetActionResult(await _mediator.Send(command));
        }

        [Route("/api/RegisterFacebook")]
        [HttpPost]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<TokenViewModel>>> RegisterFacebook([FromQuery] string AccessToken)
        {
            return GetActionResult(await _mediator.Send(new RegisterFacebookCommand { AccessToken = AccessToken }));
        }
        [Route("/api/RegisterFacebook2")]
        [HttpPost]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<TokenViewModel>>> RegisterFacebook2([FromBody] SocialPost post)
        {
            return GetActionResult(await _mediator.Send(new RegisterFacebookCommand { AccessToken = post.IdToken }));
        }

        [Route("/api/RegisterGoogle")]
        [HttpPost]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<TokenViewModel>>> RegisterGoogle([FromQuery] string IdToken)
        {
            return GetActionResult(await _mediator.Send(new RegisterGoogleCommand { IdToken = IdToken }));
        }
        [Route("/api/RegisterGoogle2")]
        [HttpPost]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<TokenViewModel>>> RegisterGoogle2([FromBody] SocialPost post)
        {
            return GetActionResult(await _mediator.Send(new RegisterGoogleCommand { IdToken = post.IdToken }));
        }
        public class SocialPost { public string IdToken { get; set; } }

        [Route("/api/RegisterApple")]
        [HttpPost]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<TokenViewModel>>> RegisterApple([FromQuery] string IdToken)
        {
            return GetActionResult(await _mediator.Send(new RegisterAppleCommand { IdToken = IdToken }));
        }
        [Route("/api/RegisterApple2")]
        [HttpPost]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<TokenViewModel>>> RegisterApple2([FromBody] SocialPost post)
        {
            return GetActionResult(await _mediator.Send(new RegisterAppleCommand { IdToken = post.IdToken, }));
        }

        [Route("/api/ForgetPassword/{email}")]
        [ApiExplorerSettings(GroupName = "fe")]
        [HttpPost]
        public async Task<ActionResult<RequestResult<bool>>> ForgetPassword(string email)
        {
            return GetActionResult(await _mediator.Send(new ForgetPasswordCommand { Email = email }));
        }

        [Route("/api/ForgetPasswordConfirm")]
        [HttpPost]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> ForgetPasswordConfirm(ForgetPasswordConfirmCommand command)
        {
            return GetActionResult(await _mediator.Send(command));
        }

        [Route("/api/RefreshToken")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<RequestResult<TokenViewModel>>> RefreshToken()
        {
            return GetActionResult(await _mediator.Send(new RefreshTokenQuery { UserId = User.Identity.Name }));
        }
    }
}
