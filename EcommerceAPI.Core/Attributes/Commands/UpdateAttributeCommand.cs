using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Attributes.Commands
{
    public class UpdateAttributeCommand : IRequest<RequestResult<bool>>
    {
        public required ProductAttributeVM ProductAttributeVM { get; set; }
        public class UpdateAttributeCommandHandler : IRequestHandler<UpdateAttributeCommand, RequestResult<bool>>
        {
            private readonly IAttributesService _attributesService;
            private readonly IUnitOfWork _unitOfWork;
            public UpdateAttributeCommandHandler(IAttributesService attributesService, IUnitOfWork unitOfWork)
            {
                _attributesService = attributesService;
                _unitOfWork = unitOfWork;
            }

            public async Task<RequestResult<bool>> Handle(UpdateAttributeCommand request, CancellationToken cancellationToken)
            {
                var dbAttribute = await _attributesService.Queryable().AsNoTracking().FirstOrDefaultAsync(x => x.ID == request.ProductAttributeVM.ID);
                if (dbAttribute == null)
                    return new RequestResult<bool>(ActionResultType.NotFound);

                dbAttribute.NameAr = request.ProductAttributeVM.NameAr;
                dbAttribute.NameEn = request.ProductAttributeVM.NameEn;
                dbAttribute.NameTr = request.ProductAttributeVM.NameTr;

                _attributesService.Update(dbAttribute);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(false);
            }
        }
    }
}
