using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.KargoCompanies.Command
{
    public class DeleteKargoCompanyCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public class DeleteKargoCompanyCommandHandler : IRequestHandler<DeleteKargoCompanyCommand, RequestResult<bool>>
        {
            private readonly IKargoCompanyService _kargoCompanyService;
            private readonly IUnitOfWork _unitOfWork;
            public DeleteKargoCompanyCommandHandler(IKargoCompanyService kargoCompanyService, IUnitOfWork unitOfWork)
            {
                _kargoCompanyService = kargoCompanyService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(DeleteKargoCompanyCommand command, CancellationToken cancellationToken)
            {
                var company = await _kargoCompanyService.Queryable().FirstOrDefaultAsync(x => x.Id == command.Id);
                if (company is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "not found");

                _kargoCompanyService.DeleteKargoCompany(company);

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "ERROR");
            }
        }
    }
}
