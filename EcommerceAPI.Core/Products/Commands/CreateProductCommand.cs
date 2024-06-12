using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.Products.Commands
{
    public class CreateProductCommand : IRequest<RequestResult<int>>
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string NameTr { get; set; }
        public string FullDescriptionAr { get; set; }
        public string FullDescriptionEn { get; set; }
        public string FullDescriptionTr { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public List<int> CategoryIds { get; set; }
        //public Vendor VendorId { get; set; }
        public double SalesPrice { get; set; }
        public double? Discount { get; set; }
        public string MainImage { get; set; }
        public List<string> Images { get; set; }
        public string SKU { get; set; }
        public string CompanyProductID { get; set; }
        public string Weight { get; set; }
        public bool IsActive { get; set; }
        public int? OrderIndex { get; set; }
        public string CompanyURL { get; set; }
        public double? CompanyPrice { get; set; }
        public string CompanyColor { get; set; }
        public List<ProductVariantVM> ProductVariants { get; set; }
        public bool OnlinePaymentOnly { get; set; }
        public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, RequestResult<int>>
        {
            private readonly IProductService _productService;
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;
            public CreateProductCommandHandler(IMapper mapper, IProductService productService, IUnitOfWork unitOfWork)
            {
                _mapper = mapper;
                _productService = productService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<int>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
            {
                var p = _mapper.Map<Product>(command);
                //p.VendorId = Vendor.Custom;
                p.IsActive = true;

                _productService.CreateProduct(p);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<int>(p.Id);
                return new RequestResult<int>(0, ActionResultType.BadRequest, "Error");
            }
        }
    }
}