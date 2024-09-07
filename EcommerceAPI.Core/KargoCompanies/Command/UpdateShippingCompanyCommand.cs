using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.ShippingCompanies.Command
{
    public class UpdateShippingCompanyCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public bool IsActive { get; set; }
        public double DeliveryLimitAmount { get; set; }
        public double DeliveryLimitFixedAmount { get; set; }
        public double DeliveryLimitPercent { get; set; }
        public int AverageDeliveryDays { get; set; }
        public class UpdateShippingCompanyCommandHandler : IRequestHandler<UpdateShippingCompanyCommand, RequestResult<bool>>
        {
            private readonly IShippingCompanyService _ShippingCompanyService;
            private readonly IUnitOfWork _unitOfWork;
            public UpdateShippingCompanyCommandHandler(IShippingCompanyService ShippingCompanyService, IUnitOfWork unitOfWork)
            {
                _ShippingCompanyService = ShippingCompanyService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(UpdateShippingCompanyCommand command, CancellationToken cancellationToken)
            {
                var company = await _ShippingCompanyService.Queryable().FirstOrDefaultAsync(x => x.Id == command.Id);
                if (company is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "not found");

                company.NameEn = command.NameEn;
                company.NameAr = command.NameAr;
                company.IsActive = command.IsActive;
                company.AverageDeliveryDays = command.AverageDeliveryDays;
                company.DeliveryLimitPercent = command.DeliveryLimitPercent;
                company.DeliveryLimitFixedAmount = command.DeliveryLimitFixedAmount;
                company.DeliveryLimitAmount = command.DeliveryLimitAmount;

                _ShippingCompanyService.UpdateShippingCompany(company);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "ERROR");
            }
        }
    }
}
