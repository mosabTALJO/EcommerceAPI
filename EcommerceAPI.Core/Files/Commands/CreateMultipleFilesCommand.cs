using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using MediatR;

namespace EcommerceAPI.Core.Files.Commands
{
    public class CreateMultipleFilesCommand : IRequest<RequestResult<string[]>>
    {
        public IFormFile[] Files { get; set; }
        public class PostMultipleFilesCommandHandler : IRequestHandler<CreateMultipleFilesCommand, RequestResult<string[]>>
        {
            private readonly IFileService _fileService;
            public PostMultipleFilesCommandHandler(IFileService fileService)
            {
                _fileService = fileService;
            }
            public async Task<RequestResult<string[]>> Handle(CreateMultipleFilesCommand command, CancellationToken cancellationToken)
            {
                var Ids = await _fileService.CreateNewFiles(command.Files);
                return Ids?.Any() == true ? new RequestResult<string[]>(Ids) : new RequestResult<string[]>(ActionResultType.OK, "Error while uploading file");
            }
        }
    }
}
