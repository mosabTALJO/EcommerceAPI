using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.CurrencyRates.Commands
{
    public class UpdateCurrencyRateCommand : IRequest<RequestResult<bool>>
    {
        public CurrencyRateVM[] Items { get; set; }

        public UpdateCurrencyRateCommand(CurrencyRateVM[] items)
        {
            Items = items;
        }

        public class UpdateCurrencyRateCommandHandler : IRequestHandler<UpdateCurrencyRateCommand, RequestResult<bool>>
        {
            private readonly ICurrencyRateService _currencyRateService;
            private readonly IUnitOfWork _unitOfWork;
            public UpdateCurrencyRateCommandHandler(ICurrencyRateService currencyRateService, IUnitOfWork unitOfWork)
            {
                _currencyRateService = currencyRateService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(UpdateCurrencyRateCommand command, CancellationToken cancellationToken)
            {
                foreach (var item in command.Items)
                {
                    var dbItem = _currencyRateService.Queryable().First(x => x.Id == item.Id);
                    //dbItem.FeeRate = item.FeeRate;
                    dbItem.Rate = item.Rate;
                    dbItem.LastUpdatedDate = DateTime.UtcNow;
                    _currencyRateService.Update(dbItem);
                }
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);

                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
