using AutoMapper;
using EcommerceAPI.Entities.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.KargoCompanies.Command
{
    public class CreateKargoCompanyCommand : IRequest<RequestResult<int>>
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public bool IsActive { get; set; }
        public double DeliveryLimitAmount { get; set; }
        public double DeliveryLimitFixedAmount { get; set; }
        public double DeliveryLimitPercent { get; set; }
        public int AverageDeliveryDays { get; set; }
        public class CreateKargoCompanyCommandHandler : IRequestHandler<CreateKargoCompanyCommand, RequestResult<int>>
        {
            private readonly IKargoCompanyService _kargoCompanyService;
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;
            public CreateKargoCompanyCommandHandler(IKargoCompanyService kargoCompanyService, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _kargoCompanyService = kargoCompanyService;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<RequestResult<int>> Handle(CreateKargoCompanyCommand command, CancellationToken cancellationToken)
            {
                var company = _mapper.Map<KargoCompany>(command);
                _kargoCompanyService.CreateKargoCompany(company);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<int>(company.Id);
                return new RequestResult<int>(ActionResultType.BadRequest, "ERROR");
            }
        }
    }
}
