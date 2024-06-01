using EcommerceAPI.Core.Files.Commands;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    public class FilesController : MyControllerBase
    {
        private readonly IFileService _fileService;
        public FilesController(IMediator mediator, IFileService fileService) : base(mediator: mediator)
        {
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<ActionResult<RequestResult<string>>> Post()
        {
            return GetActionResult(await _mediator.Send(new CreateFileCommand
            {
                NewFileName = await _fileService.CreateNewFile(Request.Form.Files.ToArray().First())
            }));
        }

        [HttpPost("Post2")]
        [Authorize]
        public async Task<ActionResult<RequestResult<string>>> Post2(IFormFile file)
        {
            return GetActionResult(await _mediator.Send(new CreateFileWithIFormFileCommand { File = file }));
        }

        [HttpPost("PostMultiple")]
        [Authorize]
        public async Task<ActionResult<RequestResult<string[]>>> PostMultiple()
        {
            return GetActionResult(await _mediator.Send(new CreateMultipleFilesCommand { Files = Request.Form.Files.ToArray() }));
        }

        [AllowAnonymous]
        [HttpGet("{image}")]
        public IActionResult DownloadImage(string image = "")
        {
            if (string.IsNullOrEmpty(image))
                return BadRequest();

            var physicalPath = _fileService.GetFilePath(image);
            return new PhysicalFileResult(physicalPath, "image/jpeg");
        }

        [AllowAnonymous]
        [HttpGet("PreviewImage/{image}")]
        public IActionResult PreviewImage(string image = "", int w = 150, int? h = null, bool crop = true)
        {
            if (string.IsNullOrEmpty(image))
                return BadRequest();

            var physicalPath = h is null ? _fileService.GetFilePath(image, w, crop) : _fileService.GetFilePath(image, w, h ?? 150, crop);
            return new PhysicalFileResult(physicalPath, "image/jpeg");
        }
    }
}
