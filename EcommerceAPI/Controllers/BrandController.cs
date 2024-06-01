using EcommerceAPI.Core.Brands.Commands;
using EcommerceAPI.Core.Brands.Queries;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    [ApiExplorerSettings(GroupName = "cp")]
    public class BrandController : MyControllerBase
    {
        public BrandController(IMediator mediator) : base(mediator: mediator)
        {
        }

        // GETList api/Brand/getlist
        [HttpGet("GetList")]
        public async Task<ActionResult<RequestResult<ListResponse<BrandVM>>>> Get(
            [FromQuery] int pageNumber = 0,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? categoryId = null,
            [FromQuery] string search = null,
            [FromQuery] string sortField = "ProductsCount",
            [FromQuery] string sortOrder = "Asc",
            [FromQuery] bool includeDeleted = false)
        {
            return GetActionResult(await _mediator.Send(new GetAllBrandsQuery
            {
                CategoryId = categoryId,
                Search = search,
                SortField = sortField,
                SortOrder = sortOrder,
                IncludeDeleted = includeDeleted,
                PageNumber = pageNumber,
                PageSize = pageSize
            }));
        }

        // Get api/Category
        [HttpGet("{id}")]
        [Authorize(Policy = PoliciesText.EditBrands)]
        public async Task<ActionResult<RequestResult<BrandVM>>> Get(int id)
        {
            return GetActionResult(await _mediator.Send(new GetBrandByIdQuery { Id = id }));
        }

        // Get api/Category
        [HttpGet("GetByCategories")]
        //[Authorize(Policy = PoliciesText.EditBrands)]
        public async Task<ActionResult<RequestResult<BrandVM[]>>> Get([FromQuery] string Categories)
        {
            var catIds = Categories.Split(",").Select(x => int.Parse(x)).ToArray();
            return GetActionResult(await _mediator.Send(new GetBrandsByCategoriesQuery { CategoriesId = catIds }));
        }

        [HttpPost]
        [Authorize(Policy = PoliciesText.EditBrands)]
        public async Task<ActionResult<RequestResult<int>>> Post([FromBody] CreateBrandCommand command)
        {
            return GetActionResult(await _mediator.Send(command));
        }

        // Put api/Brand
        [HttpPut("{Id}")]
        [Authorize(Policy = PoliciesText.EditBrands)]
        public async Task<ActionResult<RequestResult<bool>>> Put(int id, [FromBody] UpdateBrandCommand command)
        {
            command.Id = id;
            return GetActionResult(await _mediator.Send(command));
        }

        // Delete api/Brand
        [HttpDelete("{Id}")]
        [Authorize(Policy = PoliciesText.EditBrands)]
        public async Task<ActionResult<RequestResult<bool>>> Delete(int Id)
        {
            return GetActionResult(await _mediator.Send(new DeleteBrandCommand { Id = Id }));
        }
    }
}
