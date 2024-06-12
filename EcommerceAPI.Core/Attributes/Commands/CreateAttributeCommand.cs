using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.Attributes.Commands
{
    public class CreateAttributeCommand : IRequest<RequestResult<int>>
    {
        public required ProductAttributeVM ProductAttributeVM { get; set; }
        public class CreateAttributeCommandHandler : IRequestHandler<CreateAttributeCommand, RequestResult<int>>
        {
            private readonly IAttributesService _attributesService;
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;
            public CreateAttributeCommandHandler(IAttributesService attributesService, IMapper mapper, IUnitOfWork unitOfWork)
            {
                _attributesService = attributesService;
                _mapper = mapper;
                _unitOfWork = unitOfWork;
            }

            public async Task<RequestResult<int>> Handle(CreateAttributeCommand request, CancellationToken cancellationToken)
            {
                ProductAttribute attribute = _mapper.Map<ProductAttribute>(request.ProductAttributeVM);
                _attributesService.Insert(attribute);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<int>(attribute.ID);
                return new RequestResult<int>(ActionResultType.BadRequest);
            }
        }
    }
}
