using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using MediatR;

namespace EcommerceAPI.Core.Files.Commands
{
    public class CreateFileWithIFormFileCommand : IRequest<RequestResult<string>>
    {
        public IFormFile File { get; set; }
        public class CreateFileWithIFormFileCommandHandler : IRequestHandler<CreateFileWithIFormFileCommand, RequestResult<string>>
        {
            private readonly IFileService _fileService;
            public CreateFileWithIFormFileCommandHandler(IFileService fileService)
            {
                _fileService = fileService;
            }
            public async Task<RequestResult<string>> Handle(CreateFileWithIFormFileCommand command, CancellationToken cancellationToken)
            {
                var newFileName = await _fileService.CreateNewFile(command.File);
                return newFileName is null ? new RequestResult<string>(ActionResultType.BadRequest, "Error while uploading file") : new RequestResult<string>(newFileName);
            }
        }
    }
}
