using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using OfficeOpenXml;

namespace EcommerceAPI.Core.Products.Commands
{
    public class ImportFromExcelCommand : IRequest<RequestResult<string>>
    {
        public IFormFile File { get; set; }
        public class ImportFromExcelCommandHandler : IRequestHandler<ImportFromExcelCommand, RequestResult<string>>
        {
            private readonly IFileService _fileService;
            private readonly IProductService _productService;
            private readonly IBrandService _brandService;
            private readonly ICategoryService _categoryService;
            private readonly IUnitOfWork _unitOfWork;

            public ImportFromExcelCommandHandler(ICategoryService categoryService, IBrandService brandService, IProductService productService, IFileService fileService, IUnitOfWork unitOfWork)
            {
                _categoryService = categoryService;
                _brandService = brandService;
                _productService = productService;
                _fileService = fileService;
                _unitOfWork = unitOfWork;
            }

            public async Task<RequestResult<string>> Handle(ImportFromExcelCommand command, CancellationToken cancellationToken)
            {
                const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                Dictionary<int, string> columns = new Dictionary<int, string>()
            {
                {0,"NameAr"}, {1,"NameEn"}, {2,"NameTr"}, {3,"DescriptionAr"},
                {4,"DescriptionEn"}, {5,"DescriptionTr"}, {6,"FullDescriptionAr"}, {7,"FullDescriptionEn"},
                {8,"FullDescriptionTr"}, {9,"Brand"}, {10,"Category"}, {11,"SalesPrice"}, {12,"ImageURL"},
                {13,"SKU"}, {14,"CompanyProductID"}, {15,"Tags"}, {16,"Weight"},
                {17,"VendorCode"}, {18,"Color"}, {19,"Size"}, {20,"ProductID"}
            };

                var newProducts = new List<Product>();
                try
                {
                    //var file = Request?.Form?.Files?.ToArray()?.FirstOrDefault();
                    if (command.File?.Length > 0)
                    {
                        var stream = command.File.OpenReadStream();
                        using (var package = new ExcelPackage(stream))
                        {
                            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                            if (worksheet is null)
                                return new RequestResult<string>(null, ActionResultType.BadRequest, "Unable to read worksheets");

                            //validate headers
                            int column = 0;
                            while (column++ < 20)
                            {
                                if (worksheet.Cells[1, column].Value?.ToString() != columns[column - 1])
                                    return new RequestResult<string>(null, ActionResultType.BadRequest, $"Column {letters[column - 1]} must be named '{columns[column - 1]}'");
                            }

                            if (worksheet.Dimension.Rows < 2)
                                return new RequestResult<string>(null, ActionResultType.BadRequest, "The worksheet has no rows");

                            for (var row = 2; row <= worksheet.Dimension.Rows; row++)
                            {
                                try
                                {
                                    int index = 1;
                                    Product p;

                                    //Check ProductID
                                    var productID = worksheet.Cells[row, 20].Value?.ToString()?.Trim();
                                    if (!int.TryParse(productID, out int pid))
                                        return new RequestResult<string>(null, ActionResultType.BadRequest, $"ProductID has incorrect value at row {row}");

                                    bool hasParent = true;
                                    p = newProducts.FirstOrDefault(x => x.Id == pid);
                                    if (p is null)
                                    {
                                        hasParent = false;
                                        var nameAr = worksheet.Cells[row, index++].Value?.ToString()?.Trim();
                                        var nameEn = worksheet.Cells[row, index++].Value?.ToString()?.Trim();
                                        var nameTr = worksheet.Cells[row, index++].Value?.ToString()?.Trim();
                                        if (string.IsNullOrEmpty(nameAr) || string.IsNullOrEmpty(nameEn) || string.IsNullOrEmpty(nameTr))
                                            return new RequestResult<string>(null, ActionResultType.BadRequest, $"Name columns have one empty value at row {row}");

                                        var descriptionAr = worksheet.Cells[row, index++].Value?.ToString()?.Trim();
                                        var descriptionEn = worksheet.Cells[row, index++].Value?.ToString()?.Trim();
                                        var descriptionTr = worksheet.Cells[row, index++].Value?.ToString()?.Trim();
                                        if (string.IsNullOrEmpty(descriptionAr) || string.IsNullOrEmpty(descriptionEn) || string.IsNullOrEmpty(descriptionTr))
                                            return new RequestResult<string>(null, ActionResultType.BadRequest, $"Description columns have one empty value at row {row}");

                                        var fullDescriptionAr = worksheet.Cells[row, index++].Value?.ToString()?.Trim();
                                        var fullDescriptionEn = worksheet.Cells[row, index++].Value?.ToString()?.Trim();
                                        var fullDescriptionTr = worksheet.Cells[row, index++].Value?.ToString()?.Trim();
                                        if (string.IsNullOrEmpty(fullDescriptionAr) || string.IsNullOrEmpty(fullDescriptionEn) || string.IsNullOrEmpty(fullDescriptionTr))
                                            return new RequestResult<string>(null, ActionResultType.BadRequest, $"Description columns have one empty value at row {row}");

                                        var brand = worksheet.Cells[row, index++].Value?.ToString()?.Trim();
                                        if (string.IsNullOrEmpty(brand))
                                            return new RequestResult<string>(null, ActionResultType.BadRequest, $"Brand column has empty value at row {row}");

                                        var category = worksheet.Cells[row, index++].Value?.ToString()?.Trim();
                                        if (string.IsNullOrEmpty(category))
                                            return new RequestResult<string>(null, ActionResultType.BadRequest, $"Category column has empty value at row {row}");

                                        var salesPrice = worksheet.Cells[row, index++].Value?.ToString()?.Trim();
                                        if (!double.TryParse(salesPrice, out double sp))
                                            return new RequestResult<string>(null, ActionResultType.BadRequest, $"SalesPrice has incorrect value at row {row}");

                                        var ImageURL = worksheet.Cells[row, index++].Value?.ToString()?.Trim();

                                        var SKU = worksheet.Cells[row, index++].Value?.ToString()?.Trim();
                                        if (string.IsNullOrEmpty(SKU))
                                            return new RequestResult<string>(null, ActionResultType.BadRequest, $"SKU column has empty value at row {row}");
                                        if (newProducts.Any(x => x.SKU == SKU))
                                            return new RequestResult<string>(null, ActionResultType.BadRequest, $"SKU column has duplicated value at row {row}");

                                        var CompanyProductID = worksheet.Cells[row, index++].Value?.ToString()?.Trim();
                                        if (string.IsNullOrEmpty(CompanyProductID))
                                            return new RequestResult<string>(null, ActionResultType.BadRequest, $"CompanyProductID column has empty value at row {row}");

                                        var Tags = worksheet.Cells[row, index++].Value?.ToString()?.Trim();
                                        var Weight = worksheet.Cells[row, index++].Value?.ToString()?.Trim();
                                        var VendorCode = worksheet.Cells[row, index++].Value?.ToString()?.Trim();

                                        p = new Product
                                        {
                                            Id = pid,
                                            NameAr = nameAr,
                                            NameEn = nameEn,
                                            NameTr = nameTr,
                                            FullDescriptionAr = fullDescriptionAr,
                                            FullDescriptionEn = fullDescriptionEn,
                                            FullDescriptionTr = fullDescriptionTr,
                                            Brand = new Brand { Id = row, NameEn = brand },
                                            CategoryId = row,
                                            SalesPrice = sp,
                                            MainImage = ImageURL,
                                            SKU = SKU,
                                            //ProductTypes = new List<ProductType>(),
                                        };
                                    }

                                    var color = worksheet.Cells[row, index++].Value?.ToString()?.Trim();
                                    var Size = worksheet.Cells[row, index++].Value?.ToString()?.Trim();
                                    if (string.IsNullOrEmpty(color) && string.IsNullOrEmpty(Size))
                                        return new RequestResult<string>(null, ActionResultType.BadRequest, $"Color or Size must have value at row {row}");

                                    //p.ProductTypes.Add(new ProductType { Size = Size });

                                    if (!hasParent)
                                        newProducts.Add(p);
                                }
                                catch (Exception ex)
                                {
                                    return new RequestResult<string>(null, ActionResultType.BadRequest, ex.StackTrace);
                                }
                            }
                        }
                        //Check Brands
                        var allBrands = newProducts.Select(x => x.Brand.NameEn).Distinct();
                        var currentBrands = _brandService.Queryable().Where(x => allBrands.Contains(x.NameEn));
                        foreach (var b in allBrands)
                        {
                            var relatedProducts = newProducts.Where(x => x.Brand.NameEn == b);
                            var dbBrand = currentBrands.FirstOrDefault(x => x.NameEn == b);
                            if (dbBrand is null)
                                return new RequestResult<string>(null, ActionResultType.BadRequest, $"Brand doesn't exist in the database at row {relatedProducts.First().Brand.Id}");
                            relatedProducts.ToList().ForEach(x => { x.BrandId = dbBrand.Id; x.Brand.Id = dbBrand.Id; });
                        }

                        //Check Categories
                        var allCategories = newProducts.Select(x => x.Category.NameEn).Distinct();
                        var currentCategories = _categoryService.Queryable().Where(x => allCategories.Contains(x.NameEn));
                        foreach (var c in allCategories)
                        {
                            var relatedProducts = newProducts.Where(x => x.Category.NameEn == c);
                            var dbCategory = currentCategories.FirstOrDefault(x => x.NameEn == c);
                            if (dbCategory is null)
                                return new RequestResult<string>(null, ActionResultType.BadRequest, $"Category doesn't exist in the database at row {relatedProducts.First().CategoryId}");
                            relatedProducts.ToList().ForEach(x => { x.CategoryId = dbCategory.Id; });
                        }

                        //Check SKU
                        var allSKUs = newProducts.Select(x => x.SKU);
                        var existedSkus = _productService.Queryable().Where(x => allSKUs.Contains(x.SKU));
                        if (existedSkus.Any())
                            return new RequestResult<string>(null, ActionResultType.BadRequest, $"Following SKUs existed in the database: {string.Join(", ", existedSkus)}");
                    }

                    foreach (var newProduct in newProducts)
                    {
                        //if (!string.IsNullOrEmpty(newProduct.MainImage))
                        //    newProduct.MainImage = await _fileService.CreateNewFile(new Uri(newProduct.MainImage));
                        _productService.Insert(newProduct);
                    }

                    return await _unitOfWork.SaveChangesAsync() > 0
                        ? new RequestResult<string>(newProducts.Count + " product/s have been inserter")
                        : new RequestResult<string>(null, ActionResultType.BadRequest, "An error occured during saving products");
                }
                catch (Exception e)
                {
                    return new RequestResult<string>(null, ActionResultType.BadRequest, e.Message + ", " + e.InnerException?.Message);
                }
            }
        }
    }
}
