using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Attributes.Commands
{
    public class DeleteAttributeCommand : IRequest<RequestResult<bool>>
    {
        public required int ID { get; set; }
        public class DeleteAttributeCommandHandler : IRequestHandler<DeleteAttributeCommand, RequestResult<bool>>
        {
            private readonly IAttributesService _attributesService;
            private readonly IUnitOfWork _unitOfWork;
            public DeleteAttributeCommandHandler(IAttributesService attributesService, IUnitOfWork unitOfWork)
            {
                _attributesService = attributesService;
                _unitOfWork = unitOfWork;
            }

            public async Task<RequestResult<bool>> Handle(DeleteAttributeCommand request, CancellationToken cancellationToken)
            {
                if (await _attributesService.ProductVariantAttributes.AnyAsync(x => x.ProductAttributeID == request.ID))
                    return new RequestResult<bool>(ActionResultType.BadRequest, "This attribute is used by other variants!");
                var dbAttribute = await _attributesService.Queryable().AsNoTracking().FirstOrDefaultAsync(x => x.ID == request.ID);
                if (dbAttribute == null)
                    return new RequestResult<bool>(ActionResultType.NotFound);

                _attributesService.Delete(dbAttribute);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(false);
            }
        }
    }
}
