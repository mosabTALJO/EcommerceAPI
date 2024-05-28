using EcommerceAPI.Core.Gifts.Commands;
using EcommerceAPI.Core.Gifts.Queries;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    [ApiExplorerSettings(GroupName = "cp")]
    [Authorize(Policy = PoliciesText.EditGifts)]
    public class GiftsController : MyControllerBase
    {
        public GiftsController(IMediator mediator) : base(mediator: mediator)
        {
        }

        [HttpGet("{CountryId}")]
        public async Task<ActionResult<RequestResult<ListResponse<GiftPriceRangeVM>>>> Get(int CountryId)
        {
            return GetActionResult(await _mediator.Send(new GetAllGiftsQuery { CountryId = CountryId }));
        }

        [HttpPost]
        public async Task<ActionResult<RequestResult<int>>> Post(CreateGiftPriceRangeCommand command)
        {
            return GetActionResult(await _mediator.Send(command));
        }

        [HttpPut("{GiftPriceRangeId}")]
        public async Task<ActionResult<RequestResult<bool>>> Put(int GiftPriceRangeId, UpdateGiftPriceRangeCommand command)
        {
            command.Id = GiftPriceRangeId;
            return GetActionResult(await _mediator.Send(command));
        }

        [HttpDelete("{GiftPriceRangeId}")]
        public async Task<ActionResult<RequestResult<bool>>> Delete(int GiftPriceRangeId)
        {
            return GetActionResult(await _mediator.Send(new DeleteGiftPriceRangeCommand { Id = GiftPriceRangeId }));
        }
    }
}
