using EcommerceAPI.Core.Attributes.Commands;
using EcommerceAPI.Core.Attributes.Queries;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    public class AttributesController : MyControllerBase
    {
        public AttributesController(IMediator mediator) : base(mediator: mediator)
        {
        }

        [HttpGet]
        public async Task<ActionResult<RequestResult<ProductAttributeVM[]>>> GetAttributes()
        {
            return GetActionResult(await _mediator.Send(new GetAttributesQuery()));
        }

        [HttpPost]
        public async Task<ActionResult<RequestResult<int>>> Post([FromBody] ProductAttributeVM attributeVM)
        {
            return GetActionResult(await _mediator.Send(new CreateAttributeCommand { ProductAttributeVM = attributeVM }));
        }

        [HttpPut]
        public async Task<ActionResult<RequestResult<bool>>> Put([FromBody] ProductAttributeVM attributeVM)
        {
            return GetActionResult(await _mediator.Send(new UpdateAttributeCommand { ProductAttributeVM = attributeVM }));
        }

        [HttpDelete]
        public async Task<ActionResult<RequestResult<bool>>> Delete(int id)
        {
            return GetActionResult(await _mediator.Send(new DeleteAttributeCommand { ID = id }));
        }
    }
}
