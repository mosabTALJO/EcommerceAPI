using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Countries.Commands
{
    public class DeleteCountryCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, RequestResult<bool>>
        {
            private readonly ICountryService _countryService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            public DeleteCountryCommandHandler(ICountryService countryService, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _countryService = countryService;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<RequestResult<bool>> Handle(DeleteCountryCommand command, CancellationToken cancellationToken)
            {
                if (await _countryService.Queryable().Include(x=>x.Cities).AsNoTracking().FirstOrDefaultAsync(x => x.Id == command.Id) is Country dbCountry)
                {
                    if (dbCountry.IsDefault)
                        return new RequestResult<bool>(ActionResultType.BadRequest, "unable to remove the default country");

                    _countryService.DeleteCountryData(dbCountry);
                    if (await _unitOfWork.SaveChangesAsync() > 0)
                        return new RequestResult<bool>(true);
                }
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
