using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.ShippmentCompanies.Command
{
    public class DeleteShippmentCompanyCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public class DeleteShippmentCompanyCommandHandler : IRequestHandler<DeleteShippmentCompanyCommand, RequestResult<bool>>
        {
            private readonly IShippmentCompanyService _shippmentCompanyService;
            private readonly IUnitOfWork _unitOfWork;
            public DeleteShippmentCompanyCommandHandler(IShippmentCompanyService shippmentCompanyService, IUnitOfWork unitOfWork)
            {
                _shippmentCompanyService = shippmentCompanyService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(DeleteShippmentCompanyCommand command, CancellationToken cancellationToken)
            {
                var company = await _shippmentCompanyService.Queryable().FirstOrDefaultAsync(x => x.Id == command.Id);
                if (company is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "not found");

                _shippmentCompanyService.DeleteShippmentCompany(company);

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "ERROR");
            }
        }
    }
}
