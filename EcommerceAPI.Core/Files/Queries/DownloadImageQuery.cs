using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Core.Files.Queries
{
    public class DownloadImageQuery : IRequest<RequestResult<PhysicalFileResult>>
    {
        public string Image { get; set; }
        public class DownloadImageQueryHandler : IRequestHandler<DownloadImageQuery, RequestResult<PhysicalFileResult>>
        {
            private readonly IFileService _fileService;
            public DownloadImageQueryHandler(IFileService fileService)
            {
                _fileService = fileService;
            }
            public async Task<RequestResult<PhysicalFileResult>> Handle(DownloadImageQuery query, CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(query.Image))
                    return new RequestResult<PhysicalFileResult>(ActionResultType.BadRequest);

                var physicalPath = _fileService.GetFilePath(query.Image);
                var result = new PhysicalFileResult(physicalPath, "image/jpeg");
                return new RequestResult<PhysicalFileResult>(result);
            }
        }
    }
}
