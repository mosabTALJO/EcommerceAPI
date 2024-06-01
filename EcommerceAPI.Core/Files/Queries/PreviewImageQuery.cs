using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Core.Files.Queries
{
    public class PreviewImageQuery : IRequest<RequestResult<PhysicalFileResult>>
    {
        public string Image { get; set; } = "";
        public int W { get; set; } = 150;
        public int H { get; set; } = 150;
        public bool Crop { get; set; } = true;
        public class PreviewImageQueryHandler : IRequestHandler<PreviewImageQuery, RequestResult<PhysicalFileResult>>
        {
            private readonly IFileService _fileService;
            public PreviewImageQueryHandler(IFileService fileService)
            {
                _fileService = fileService;
            }
            public async Task<RequestResult<PhysicalFileResult>> Handle(PreviewImageQuery query, CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(query.Image))
                    return new RequestResult<PhysicalFileResult>(ActionResultType.BadRequest);

                var physicalPath = _fileService.GetFilePath(query.Image, query.W, query.H, query.Crop);
                var result = new PhysicalFileResult(physicalPath, "image/jpeg");
                return new RequestResult<PhysicalFileResult>(result);
            }
        }
    }
}
