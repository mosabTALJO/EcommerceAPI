using EcommerceAPI.Core.Filterings.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EcommerceAPI.Web.Controllers
{
    public class FilteringController : MyControllerBase
    {
        public FilteringController(IMediator mediator) : base(mediator: mediator)
        {
        }

        [HttpGet]
        public async Task<ActionResult<Filtering[]>> GetProducts(
            [FromQuery] int? categoryId = null,
            [FromQuery] int? collectionId = null,
            [FromQuery] int? brandId = null,
            [FromQuery] string search = null)
        {
            return Ok(await _mediator.Send(new GetProductsFilteringQuery
            {
                CategoryId = categoryId,
                CollectionId = collectionId,
                BrandId = brandId,
                Search = search
            }));
        }
    }
}
