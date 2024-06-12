using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.UnitOfWork.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EcommerceAPI.Web.Controllers
{
    [ApiExplorerSettings(GroupName = "cp")]
    public class CollectionController : MyControllerBase
    {
        private readonly ICollectionService _collectionService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrencyRateService _currencyRateService;

        public CollectionController(ICollectionService collectionService, IProductService productService, IMapper mapper, IUnitOfWork unitOfWork, ICurrencyRateService currencyRateService) : base()
        {
            _collectionService = collectionService;
            _productService = productService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _currencyRateService = currencyRateService;
        }

        // GETList api/Collection/
        [HttpGet]
        [Authorize(Policy = PoliciesText.EditCollections)]
        public async Task<ActionResult<ListResponse<CollectionVM>>> Get(
            [FromQuery] int pageNumber = 0,
            [FromQuery] int pageSize = 20,
            [FromQuery] string search = null
            )
        {
            var lst = _collectionService.Queryable();
            if (!string.IsNullOrEmpty(search))
                lst = lst.Where(x => x.Name.Contains(search));

            var result = new ListResponse<CollectionVM>
            {
                TotalRecords = await lst.CountAsync(),
                Items = await lst.OrderBy(x => x.Id).Skip(pageSize * pageNumber).Take(pageSize)
                    .To<CollectionVM>(_mapper).ToArrayAsync()
            };
            return Ok(result);
        }

        // GETList api/Collection/ID
        [HttpGet("{Id}")]
        [Authorize(Policy = PoliciesText.EditCollections)]
        public async Task<ActionResult<ListResponse<CollectionVM>>> Get(int Id)
        {
            var collection = _collectionService.Queryable().Include(x => x.Items).To<CollectionVM>(_mapper).FirstOrDefault(x => x.Id == Id);
            var lang = CultureInfo.CurrentCulture?.TwoLetterISOLanguageName ?? "en";
            var currencyRate = _currencyRateService.GetCurrencyRate(Request.Headers["Currency"]);
            var productIds = collection.Items.Select(x => x.ProductId).Distinct().ToArray();
            var products = await _productService.Queryable().Include(x => x.Category)
                .Where(x => productIds.Contains(x.Id) && x.Category.IsActive)
                .Select(x => new ProductLiteVM
                {
                    Name = lang == "en" ? x.NameEn : lang == "ar" ? x.NameAr : x.NameTr,
                    BrandName = lang == "en" ? x.Brand.NameEn : lang == "ar" ? x.Brand.NameAr : x.Brand.NameTr,
                    MainImageRow = x.MainImage,
                    Id = x.Id,
                    SalesPrice = x.SalesPrice * currencyRate,
                    Discount = x.Discount > 0 ? x.Discount : x.Category.Discount > 0 ? x.Category.Discount : null,
                    TotalRate = x.TotalRate,
                    CreatedAt = x.CreatedAt,
                }).ToArrayAsync();

            foreach (var item in collection.Items)
                item.Product = products.FirstOrDefault(x => x.Id == item.ProductId);

            collection.Items = collection.Items.Where(x => x.Product != null).OrderBy(x => x.OrderIndex).ToArray();
            return Ok(collection);
        }

        [HttpPost]
        [Authorize(Policy = PoliciesText.EditCollections)]
        public async Task<ActionResult<int>> Post([FromBody] CollectionVM item)
        {
            item.Id = 0;
            _collectionService.Insert(_mapper.Map<Collection>(item));
            if (await _unitOfWork.SaveChangesAsync() > 0)
                return Ok(true);
            return BadRequest("Error");
        }

        // Put api/Brand
        [HttpPut("{id}")]
        [Authorize(Policy = PoliciesText.EditCollections)]
        public async Task<ActionResult<bool>> Put(int id, [FromBody] CollectionVM item)
        {
            var c = await _collectionService.Queryable().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (c is null)
                return NotFound("IdError");
            c.Name = item.Name?.Trim();

            _collectionService.UpdateCollection(c, item.Items.Select(x => _mapper.Map<CollectionItem>(x)).ToArray());
            if (await _unitOfWork.SaveChangesAsync() > 0)
                return Ok(true);
            return BadRequest("Error");
        }

        // Delete api/Brand
        [HttpDelete("{Id}")]
        [Authorize(Policy = PoliciesText.EditCollections)]
        public async Task<IActionResult> Delete(int Id)
        {
            if (await _collectionService.FindAsync(Id) is Collection item)
            {
                _collectionService.Delete(item);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return Ok(true);
            }
            return BadRequest("Error");
        }
    }
}
