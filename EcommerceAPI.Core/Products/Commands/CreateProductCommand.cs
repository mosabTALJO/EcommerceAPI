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
        public string CurrencyCode { get; set; } = string.Empty;
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string? NameTr { get; set; }
        public string FullDescriptionAr { get; set; }
        public string FullDescriptionEn { get; set; }
        public string FullDescriptionTr { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public double SalesPrice { get; set; }
        public double? Discount { get; set; }
        public string MainImage { get; set; }
        public List<string> Images { get; set; }
        public string SKU { get; set; }
        public bool IsActive { get; set; }
        public int? OrderIndex { get; set; }
        public List<ProductVariantVM>? ProductVariants { get; set; }
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
                //fix images
                foreach (var pv in command.ProductVariants ?? [])
                    pv.Images = pv.Images.Intersect(command.Images).ToList();
                command.NameTr ??= command.NameEn;

                var p = _mapper.Map<Product>(command);
                p.IsActive = true;

                _productService.CreateProduct(p);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<int>(p.Id);
                return new RequestResult<int>(0, ActionResultType.BadRequest, "Error");
            }
        }
    }
}