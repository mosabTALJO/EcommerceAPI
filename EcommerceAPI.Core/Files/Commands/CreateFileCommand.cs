using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using MediatR;

namespace EcommerceAPI.Core.Files.Commands
{
    public class CreateFileCommand : IRequest<RequestResult<string>>
    {
        public string NewFileName { get; set; }
        public class CreateFileCommandHandler : IRequestHandler<CreateFileCommand, RequestResult<string>>
        {
            private readonly IFileService _fileService;

            public CreateFileCommandHandler(IFileService fileService)
            {
                _fileService = fileService;
            }
            public async Task<RequestResult<string>> Handle(CreateFileCommand command, CancellationToken cancellationToken)
            {
                return command.NewFileName is null ? new RequestResult<string>(ActionResultType.BadRequest, "Error while uploading file") : new RequestResult<string>(command.NewFileName);
            }
        }
    }
}
