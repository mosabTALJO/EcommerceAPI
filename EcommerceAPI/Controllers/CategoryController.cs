using EcommerceAPI.Core.Categories.Commands;
using EcommerceAPI.Core.Categories.Queries;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    [ApiExplorerSettings(GroupName = "cp")]
    public class CategoryController : MyControllerBase
    {
        public CategoryController(IMediator mediator) : base(mediator: mediator)
        {
        }

        // GETList api/Category
        [HttpGet]
        [Authorize(Policy = PoliciesText.EditCategories)]
        public async Task<ActionResult<RequestResult<ListResponse<CategoryVM>>>> Get(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string search = null,
            [FromQuery] string sortField = "Id",
            [FromQuery] string sortOrder = "Asc")
        {
            return GetActionResult(await _mediator.Send(new GetAllCategoriesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = search,
                SortField = sortField,
                SortOrder = sortOrder
            }));
        }

        // GETList api/Category
        [HttpGet("GetAll")]
        public async Task<ActionResult<RequestResult<CategoryLite[]>>> Get()
        {
            return GetActionResult(await _mediator.Send(new GetAllCategoriesLiteQuery()));
        }

        [HttpGet("GetParents")]
        [AllowAnonymous]
        public async Task<ActionResult<RequestResult<CategoryLite[]>>> GetParents()
        {
            return GetActionResult(await _mediator.Send(new GetCategoryParentsQuery()));
        }


        // Get api/Category
        [HttpGet("{id}")]
        [Authorize(Policy = PoliciesText.EditCategories)]
        public async Task<ActionResult<RequestResult<CategoryVM>>> Get(int id)
        {
            return GetActionResult(await _mediator.Send(new GetCategoryByIdQuery { Id = id }));
        }


        [HttpPost]
        [Authorize(Policy = PoliciesText.EditCategories)]
        public async Task<ActionResult<RequestResult<bool>>> Post([FromBody] CreateCategoryCommand command)
        {
            return GetActionResult(await _mediator.Send(command));
        }

        // Put api/Category
        [HttpPut("{id}")]
        [Authorize(Policy = PoliciesText.EditCategories)]
        public async Task<ActionResult<RequestResult<bool>>> Put(int id, [FromBody] UpdateCategoryCommand command)
        {
            command.Id = id;
            return GetActionResult(await _mediator.Send(command));
        }

        // Delete api/Category
        [HttpDelete("{Id}")]
        [Authorize(Policy = PoliciesText.EditCategories)]
        public async Task<ActionResult<RequestResult<bool>>> Delete(int Id)
        {
            return GetActionResult(await _mediator.Send(new DeleteCategoryCommand { Id = Id }));
        }

        [HttpPut("OrderItems")]
        [Authorize(Policy = PoliciesText.EditCategories)]
        public async Task<ActionResult<RequestResult<bool>>> OrderItems([FromBody] CategoryOrdering[] items)
        {
            var command = new OrderCategoriesCommand(items);
            return GetActionResult(await _mediator.Send(command));
        }
    }
}
