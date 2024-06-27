using EcommerceAPI.Core.Products.Commands;
using EcommerceAPI.Core.Products.Queries;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace EcommerceAPI.Web.Controllers
{
    public class ProductController : MyControllerBase
    {
        public readonly IMemoryCache _memoryCache;
        public ProductController(IMediator mediator, IMemoryCache memoryCache) : base(mediator: mediator)
        {
            _memoryCache = memoryCache;
        }

        // GETList api/Product
        [HttpGet("GetProducts")]
        [HttpGet]
        public async Task<ActionResult<RequestResult<ListResponse<ProductLiteVM>>>> GetProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? categoryId = null,
            [FromQuery] int? collectionId = null,
            [FromQuery] int? brandId = null,
            [FromQuery] string search = null,

            [FromQuery] string brands = null,
            [FromQuery] string sizes = null,
            [FromQuery] string colors = null,
            [FromQuery] string gender = null,
            [FromQuery] double? minPrice = null,
            [FromQuery] double? maxPrice = null,
            [FromQuery] bool showHidden = false,
            [FromQuery] bool showLatestFirst = false //TODO
            )
        {
            return GetActionResult(await _mediator.Send(new GetProductsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                CategoryId = categoryId,
                CollectionId = collectionId,
                BrandId = brandId,
                Search = search,
                Brands = brands,
                Colors = colors,
                Gender = gender,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                ShowHidden = showHidden,
                UserId = User.Identity.Name,
                Currency = Request.Headers["Currency"]
            }));
        }

        [HttpGet("GetRelatedProducts/{id}")]
        public async Task<ActionResult<RequestResult<ListResponse<ProductLiteVM>>>> GetRelatedProducts(int id)
        {
            return GetActionResult(await _mediator.Send(new GetRelatedProductsQuery
            {
                Id = id,
                UserId = User.Identity.Name,
                Currency = Request.Headers["Currency"],
                ClaimValue = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RequestResult<ProductVM>>> Get(int id)
        {
            return GetActionResult(await _mediator.Send(new GetProductDetailsQuery
            {
                Id = id,
                UserId = User.Identity.Name,
                Role = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
                Currency = Request.Headers["Currency"]
            }));
        }

        [HttpGet("GetProductDetails/{id}")]
        public async Task<ActionResult<RequestResult<ProductVM>>> GetProductDetails(int id)
        {
            return GetActionResult(await _mediator.Send(new GetProductDetailsQuery
            {
                Id = id,
                Currency = Request.Headers["Currency"],
                UserId = User.Identity.Name,
                Role = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
            }));
        }

        [HttpGet("GetProductTypes/{id}")]
        public async Task<ActionResult<RequestResult<ProductVariantVM[]>>> GetProductVariants(int id)
        {
            return GetActionResult(await _mediator.Send(new GetProductVariantsQuery
            {
                Id = id,
            }));
        }

        [HttpPost]
        [Authorize(Policy = PoliciesText.CreateProduct)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<int>>> Post([FromBody] CreateProductCommand command)
        {
            return GetActionResult(await _mediator.Send(command));
        }

        [HttpPut("{Id}")]
        [Authorize(Policy = PoliciesText.UpdateProduct)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> Put(int id, [FromBody] UpdateProductCommand command)
        {
            command.Id = id;
            return GetActionResult(await _mediator.Send(command));
        }

        [HttpDelete("{Id}")]
        [Authorize(Policy = PoliciesText.DeleteProduct)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> Delete(int Id)
        {
            return GetActionResult(await _mediator.Send(new DeleteProductCommand { Id = Id }));
        }

        [HttpPut("Hide/{Id}")]
        [Authorize(Policy = PoliciesText.UpdateProduct)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> Hide(int Id)
        {
            return GetActionResult(await _mediator.Send(new HideProductCommand { Id = Id }));
        }

        [HttpPut("UnHide/{Id}")]
        [Authorize(Policy = PoliciesText.UpdateProduct)]
        [ApiExplorerSettings(GroupName = "cp")]
        public async Task<ActionResult<RequestResult<bool>>> UnHide(int Id)
        {
            return GetActionResult(await _mediator.Send(new UnHideProductCommand { Id = Id }));
        }

        //[HttpPost("PostFile")]
        //[Authorize(Policy = PoliciesText.CreateProduct)]
        //[ApiExplorerSettings(GroupName = "cp")]
        //public async Task<ActionResult<RequestResult<string>>> ImportFromExcel()
        //{
        //    return GetActionResult(await _mediator.Send(new ImportFromExcelCommand { File = Request?.Form?.Files?.ToArray()?.FirstOrDefault() }));
        //}


        //[HttpPost("PostFile1")]
        //[Authorize(Policy = PoliciesText.CreateProduct)]
        //[ApiExplorerSettings(GroupName = "xx")]
        //public async Task<ActionResult<RequestResult<bool>>> PostFile1()
        //{
        //    return GetActionResult(await _mediator.Send(new CreateFileCommand { File = Request?.Form?.Files?.ToArray()?.FirstOrDefault() }));
        //}
    }
}