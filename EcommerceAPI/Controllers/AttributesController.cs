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
        public async Task<ActionResult<RequestResult<ProductAttributeVM[]>>> GetAttributes([FromQuery] string? search = null) =>
            GetActionResult(await _mediator.Send(new GetAttributesQuery() { search = search }));

        [HttpGet("GetAttributeValues/{id}")]
        public async Task<ActionResult<RequestResult<string[]>>> GetAttributeValues(int id) =>
            GetActionResult(await _mediator.Send(new GetAttributeValuesQuery { Id = id }));

        [HttpPost]
        public async Task<ActionResult<RequestResult<int>>> Post([FromBody] CreateAttributeCommand attributeVM) =>
            GetActionResult(await _mediator.Send(attributeVM));

        [HttpPut]
        public async Task<ActionResult<RequestResult<bool>>> Put([FromBody] ProductAttributeVM attributeVM) =>
            GetActionResult(await _mediator.Send(new UpdateAttributeCommand { ProductAttributeVM = attributeVM }));

        [HttpDelete]
        public async Task<ActionResult<RequestResult<bool>>> Delete(int id) =>
            GetActionResult(await _mediator.Send(new DeleteAttributeCommand { ID = id }));
    }
}