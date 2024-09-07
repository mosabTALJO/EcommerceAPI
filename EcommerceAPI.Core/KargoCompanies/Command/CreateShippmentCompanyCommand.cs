using AutoMapper;
using EcommerceAPI.Entities.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.ShippmentCompanies.Command
{
    public class CreateShippmentCompanyCommand : IRequest<RequestResult<int>>
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public bool IsActive { get; set; }
        public double DeliveryLimitAmount { get; set; }
        public double DeliveryLimitFixedAmount { get; set; }
        public double DeliveryLimitPercent { get; set; }
        public int AverageDeliveryDays { get; set; }
        public class CreateShippmentCompanyCommandHandler : IRequestHandler<CreateShippmentCompanyCommand, RequestResult<int>>
        {
            private readonly IShippmentCompanyService _shippmentCompanyService;
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;
            public CreateShippmentCompanyCommandHandler(IShippmentCompanyService shippmentCompanyService, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _shippmentCompanyService = shippmentCompanyService;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<RequestResult<int>> Handle(CreateShippmentCompanyCommand command, CancellationToken cancellationToken)
            {
                var company = _mapper.Map<ShippmentCompany>(command);
                _shippmentCompanyService.CreateShippmentCompany(company);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<int>(company.Id);
                return new RequestResult<int>(ActionResultType.BadRequest, "ERROR");
            }
        }
    }
}
