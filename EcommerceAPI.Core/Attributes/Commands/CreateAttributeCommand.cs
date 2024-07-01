using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.Attributes.Commands
{
    public class CreateAttributeCommand : IRequest<RequestResult<int>>
    {
        public required string Name { get; set; }
        public string? NameAr { get; set; }
        public string? NameTr { get; set; }
        public string? NameEn { get; set; }
        public class CreateAttributeCommandHandler : IRequestHandler<CreateAttributeCommand, RequestResult<int>>
        {
            private readonly IAttributesService _attributesService;
            private readonly IUnitOfWork _unitOfWork;
            public CreateAttributeCommandHandler(IAttributesService attributesService, IUnitOfWork unitOfWork)
            {
                _attributesService = attributesService;
                _unitOfWork = unitOfWork;
            }

            public async Task<RequestResult<int>> Handle(CreateAttributeCommand request, CancellationToken cancellationToken)
            {
                ProductAttribute attribute = new ProductAttribute
                {
                    NameEn = string.IsNullOrEmpty(request.NameEn) ? request.Name : request.NameEn,
                    NameAr = string.IsNullOrEmpty(request.NameAr) ? request.Name : request.NameAr,
                    NameTr = string.IsNullOrEmpty(request.NameTr) ? request.Name : request.NameTr,
                };

                _attributesService.Insert(attribute);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<int>(attribute.ID);
                return new RequestResult<int>(ActionResultType.BadRequest);
            }
        }
    }
}
