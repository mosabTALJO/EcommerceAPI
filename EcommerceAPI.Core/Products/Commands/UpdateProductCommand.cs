using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Products.Commands
{
    public class UpdateProductCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string NameTr { get; set; }
        public string FullDescriptionAr { get; set; }
        public string FullDescriptionEn { get; set; }
        public string FullDescriptionTr { get; set; }
        public int BrandId { get; set; }
        public BrandVM Brand { get; set; }
        public int? CategoryId { get; set; }
        public int[] CategoryIds { get; set; }
        public double SalesPrice { get; set; }
        public double? Discount { get; set; }
        public string MainImage { get; set; }
        public List<string> Images { get; set; }
        public string SKU { get; set; }
        public string Weight { get; set; }
        public bool IsActive { get; set; }
        public int? OrderIndex { get; set; }
        public List<ProductVariantVM> ProductVariants { get; set; }
        public bool OnlinePaymentOnly { get; set; }
        public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, RequestResult<bool>>
        {
            private readonly IProductService _productService;
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;

            public UpdateProductCommandHandler(IMapper mapper, IProductService productService, IUnitOfWork unitOfWork)
            {
                _mapper = mapper;
                _productService = productService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
            {
                if (await _productService.Queryable().AsNoTracking().FirstOrDefaultAsync(x => x.Id == command.Id) is Product p)
                {
                    if (p.DeletedAt.HasValue)
                        return new RequestResult<bool>(false, ActionResultType.BadRequest, "DeletedItem");

                    //remove main image from the list
                    command.Images = command.Images?.Where(x => x != command.MainImage && x != "").ToList();
                    p.NameAr = command.NameAr;
                    p.NameEn = command.NameEn;
                    p.NameTr = command.NameTr;
                    p.FullDescriptionAr = command.FullDescriptionAr;
                    p.FullDescriptionEn = command.FullDescriptionEn;
                    p.FullDescriptionTr = command.FullDescriptionTr;
                    p.BrandId = command.BrandId;
                    p.CategoryId = command.CategoryId ?? command.CategoryIds?.FirstOrDefault() ?? p.CategoryId;
                    p.Discount = command.Discount;
                    p.Images = command.Images?.Count > 0 ? string.Join(',', command.Images) : "";
                    p.MainImage = command.MainImage;
                    p.SalesPrice = command.SalesPrice;
                    p.IsActive = command.IsActive;
                    p.OnlinePaymentOnly = command.OnlinePaymentOnly;
                    p.ProductVariants = command.ProductVariants.Select(_mapper.Map<ProductVariant>).ToList();

                    _productService.UpdateProduct(p);
                    if (await _unitOfWork.SaveChangesAsync() > 0)
                        return new RequestResult<bool>(true);
                }
                else
                    return new RequestResult<bool>(false, ActionResultType.NotFound, "IdError");
                return new RequestResult<bool>(false, ActionResultType.BadRequest, "Error");
            }
        }
    }
}
