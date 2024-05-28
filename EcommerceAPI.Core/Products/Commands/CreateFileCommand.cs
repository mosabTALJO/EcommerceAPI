//using EcommerceAPI.Shared.Helpers;
//using EcommerceAPI.UnitOfWork.Abstractions;
//using MediatR;
//using OfficeOpenXml;
//using Microsoft.AspNetCore.Http;

//namespace EcommerceAPI.Core.Products.Commands
//{
//    public class CreateFileCommand : IRequest<RequestResult<bool>>
//    {
//        public IFormFile? File { get; set; }
//        public class CreateFileCommandHandler : IRequestHandler<CreateFileCommand, RequestResult<bool>>
//        {
//            private readonly IUnitOfWork _unitOfWork;
//            public CreateFileCommandHandler(IUnitOfWork unitOfWork)
//            {
//                _unitOfWork = unitOfWork;
//            }
//            public async Task<RequestResult<bool>> Handle(CreateFileCommand command, CancellationToken cancellationToken)
//            {
//                try
//                {
//                    var lst = new Dictionary<int, List<string>>();
//                    //var file = Request?.Form?.Files?.ToArray()?.FirstOrDefault();
//                    if (command.File?.Length > 0)
//                    {
//                        var stream = command.File.OpenReadStream();
//                        using (var package = new ExcelPackage(stream))
//                        {
//                            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
//                            if (worksheet is null)
//                                return new RequestResult<bool>(false, ActionResultType.BadRequest, "Unable to read worksheets");

//                            if (worksheet.Dimension.Rows < 2)
//                                return new RequestResult<bool>(false, ActionResultType.BadRequest, "The worksheet has no rows");

//                            for (var row = 2; row <= worksheet.Dimension.Rows; row++)
//                            {
//                                try
//                                {
//                                    //Check ProductID
//                                    var categoryId = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
//                                    if (!int.TryParse(categoryId, out int cid))
//                                        return new RequestResult<bool>(false, ActionResultType.BadRequest, $"CategoryId has incorrect value at row {row}");

//                                    var url = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
//                                    if (lst.ContainsKey(cid))
//                                        lst[cid].Add(url);
//                                    else
//                                        lst.Add(cid, new List<string> { url });
//                                }
//                                catch (Exception ex)
//                                {
//                                    return new RequestResult<bool>(false, ActionResultType.BadRequest, ex.StackTrace);
//                                }
//                            }
//                        }
//                    }
//                    return new RequestResult<bool>(await _unitOfWork.SaveChangesAsync() > 0);
//                }
//                catch (Exception e)
//                {
//                    return new RequestResult<bool>(false, ActionResultType.BadRequest, e.Message + ", " + e.InnerException?.Message);
//                }
//            }
//        }
//    }
//}
