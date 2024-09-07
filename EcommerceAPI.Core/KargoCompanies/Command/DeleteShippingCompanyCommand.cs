using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.ShippingCompanies.Command
{
    public class DeleteShippingCompanyCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public class DeleteShippingCompanyCommandHandler : IRequestHandler<DeleteShippingCompanyCommand, RequestResult<bool>>
        {
            private readonly IShippingCompanyService _ShippingCompanyService;
            private readonly IUnitOfWork _unitOfWork;
            public DeleteShippingCompanyCommandHandler(IShippingCompanyService ShippingCompanyService, IUnitOfWork unitOfWork)
            {
                _ShippingCompanyService = ShippingCompanyService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(DeleteShippingCompanyCommand command, CancellationToken cancellationToken)
            {
                var company = await _ShippingCompanyService.Queryable().FirstOrDefaultAsync(x => x.Id == command.Id);
                if (company is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "not found");

                _ShippingCompanyService.DeleteShippingCompany(company);

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "ERROR");
            }
        }
    }
}
