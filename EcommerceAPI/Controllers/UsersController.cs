using EcommerceAPI.Core.Users.Command;
using EcommerceAPI.Core.Users.Queries;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;

namespace EcommerceAPI.Web.Controllers
{
    public class UsersController : MyControllerBase
    {
        private readonly ILogger<UsersController> logger;
        public UsersController(IMediator mediator, ILogger<UsersController> logger) : base(mediator)
        {
            this.logger = logger;
        }

        // PUT api/Users/5
        [HttpPut("UpdateUser")]
        [Authorize]
        public async Task<ActionResult<RequestResult<bool>>> UpdateUser([FromBody] UpdateUserCommand command)
        {
            command.UserId = User.Identity.Name;
            return GetActionResult(await _mediator.Send(command));
        }

        //// GET api/users/GetMyInfo
        [HttpGet("GetMyInfo")]
        [Authorize]
        public async Task<ActionResult<RequestResult<UserVM>>> GetMyInfo()
        {
            return GetActionResult(await _mediator.Send(new GetMyUserInfoQuery { UserId = User.Identity.Name }));
        }

        // GET api/users
        [HttpGet]
        [Authorize(Policy = PoliciesText.UsersPage)]
        [Route("GetAllUsers")]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<ListResponse<UserVM>>>> Get(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string search = null,
            [FromQuery] string sortField = "CreatedAt",
            [FromQuery] string sortOrder = "Asc")
        {
            return GetActionResult(await _mediator.Send(new GetAllUsersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = search,
                SortField = sortField,
                SortOrder = sortOrder
            }));
        }

        // GET api/users
        [HttpGet("GetUserInfo/{id}")]
        [Authorize(Policy = PoliciesText.UsersPage)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<UserVM>>> Get(string id)
        {
            return GetActionResult(await _mediator.Send(new GetUserInfoQuery { Id = id }));
        }

        [Route("CreateAdminUser")]
        [HttpPost]
        [Authorize(Policy = PoliciesText.EditAdmins)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<string>>> CreateAdminUser([FromBody] CreateAdminUserCommand command)
        {
            command.UserId = User.Identity.Name;
            return GetActionResult(await _mediator.Send(command));
        }

        // PUT api/Users/5
        [HttpPut("UpdateAdminUser/{id}")]
        [Authorize(Policy = PoliciesText.EditAdmins)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> UpdateAdminUser(string id, [FromBody] UpdateAdminUserCommand command)
        {
            command.Id = id;
            command.UserId = User.Identity!.Name!;
            return GetActionResult(await _mediator.Send(command));
        }

        // GET api/users
        [HttpDelete("DeleteAdminUser/{id}")]
        [Authorize(Policy = PoliciesText.EditAdmins)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> DeleteAdminUser(string id)
        {
            return GetActionResult(await _mediator.Send(new DeleteAdminUserCommand { Id = id, UserId = User.Identity!.Name! }));
        }

        // GET api/users
        [HttpDelete("DeleteUserData")]
        [Authorize(Roles = "Customer")]
        [ApiExplorerSettings(GroupName = "fe")]
        public async Task<ActionResult<RequestResult<bool>>> DeleteUserData()
        {
            var xx = await _mediator.Send(new DeleteUserDataCommand { Id = User.Identity.Name });
            if (!xx.Success)
                logger.LogError(xx.ErrorText);
            return GetActionResult(xx);
        }

        [HttpGet("GetDefaultCountry")]
        public async Task<ActionResult<string>> GetDefaultCountry()
        {
            var clientIpAddress = GetClientIP(Request);
            try
            {
                var r = await new HttpClient().GetAsync("https://ipinfo.io/" + clientIpAddress + "?token=4723ac3b4a6c19");
                var ipData = JsonSerializer.Deserialize<IpData>(await r.Content.ReadAsStringAsync());
                return Ok(new CultureInfo(ipData.country)?.ThreeLetterISOLanguageName);
            }
            catch (Exception ex)
            {
                return BadRequest("Unable to find the country from: " + clientIpAddress);
            }
        }
        public static string GetClientIP(HttpRequest request)
        {
            try
            {
                string _ipList = request.Headers["CF-CONNECTING-IP"].ToString();
                if (!string.IsNullOrWhiteSpace(_ipList))
                {
                    return _ipList.Split(',')[0].Trim();
                }
                else
                {
                    return request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                }
            }
            catch { return null; }
        }
        public class IpData
        {
            //public string ip { get; set; }
            //public string city { get; set; }
            //public string region { get; set; }
            public string country { get; set; }
            //public string loc { get; set; }
            //public string org { get; set; }
            //public string postal { get; set; }
            //public string timezone { get; set; }
            //public string readme { get; set; }
        }
    }
}