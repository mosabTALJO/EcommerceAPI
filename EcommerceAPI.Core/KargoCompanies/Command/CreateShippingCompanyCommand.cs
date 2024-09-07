using AutoMapper;
using EcommerceAPI.Entities.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.ShippingCompanies.Command
{
    public class CreateShippingCompanyCommand : IRequest<RequestResult<int>>
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public bool IsActive { get; set; }
        public double DeliveryLimitAmount { get; set; }
        public double DeliveryLimitFixedAmount { get; set; }
        public double DeliveryLimitPercent { get; set; }
        public int AverageDeliveryDays { get; set; }
        public class CreateShippingCompanyCommandHandler : IRequestHandler<CreateShippingCompanyCommand, RequestResult<int>>
        {
            private readonly IShippingCompanyService _ShippingCompanyService;
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;
            public CreateShippingCompanyCommandHandler(IShippingCompanyService ShippingCompanyService, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _ShippingCompanyService = ShippingCompanyService;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<RequestResult<int>> Handle(CreateShippingCompanyCommand command, CancellationToken cancellationToken)
            {
                var company = _mapper.Map<ShippingCompany>(command);
                _ShippingCompanyService.CreateShippingCompany(company);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<int>(company.Id);
                return new RequestResult<int>(ActionResultType.BadRequest, "ERROR");
            }
        }
    }
}
