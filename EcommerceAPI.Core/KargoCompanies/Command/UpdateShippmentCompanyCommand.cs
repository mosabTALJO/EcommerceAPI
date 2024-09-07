using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.ShippmentCompanies.Command
{
    public class UpdateShippmentCompanyCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public bool IsActive { get; set; }
        public double DeliveryLimitAmount { get; set; }
        public double DeliveryLimitFixedAmount { get; set; }
        public double DeliveryLimitPercent { get; set; }
        public int AverageDeliveryDays { get; set; }
        public class UpdateShippmentCompanyCommandHandler : IRequestHandler<UpdateShippmentCompanyCommand, RequestResult<bool>>
        {
            private readonly IShippmentCompanyService _shippmentCompanyService;
            private readonly IUnitOfWork _unitOfWork;
            public UpdateShippmentCompanyCommandHandler(IShippmentCompanyService shippmentCompanyService, IUnitOfWork unitOfWork)
            {
                _shippmentCompanyService = shippmentCompanyService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(UpdateShippmentCompanyCommand command, CancellationToken cancellationToken)
            {
                var company = await _shippmentCompanyService.Queryable().FirstOrDefaultAsync(x => x.Id == command.Id);
                if (company is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "not found");

                company.NameEn = command.NameEn;
                company.NameAr = command.NameAr;
                company.IsActive = command.IsActive;
                company.AverageDeliveryDays = command.AverageDeliveryDays;
                company.DeliveryLimitPercent = command.DeliveryLimitPercent;
                company.DeliveryLimitFixedAmount = command.DeliveryLimitFixedAmount;
                company.DeliveryLimitAmount = command.DeliveryLimitAmount;

                _shippmentCompanyService.UpdateShippmentCompany(company);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "ERROR");
            }
        }
    }
}
