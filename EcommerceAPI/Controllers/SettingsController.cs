using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Controllers
{
    [ApiExplorerSettings(GroupName = "cp")]
    [Authorize(Policy = PoliciesText.CleaningDatabase)]
    public class SettingsController : MyControllerBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryService _categoryService;
        public SettingsController(ISettingsService settingsService, IUnitOfWork unitOfWork, ICategoryService categoryService) : base()
        {
            _settingsService = settingsService;
            _unitOfWork = unitOfWork;
            _categoryService = categoryService;
        }

        [HttpGet("CanCleanDatabase")]
        public ActionResult<bool> CanCleanDatabase() =>
           Ok(_settingsService.Queryable().FirstOrDefault(x => x.Name == "IsCleaning")?.Value != "1");

        [HttpPost("CleanDatabase/{CategoryId}")]
        public async Task<ActionResult<bool>> CleanDatabase(int CategoryId = 0)
        {
            var record = _settingsService.Queryable().FirstOrDefault(x => x.Name == "IsCleaning") ?? new Settings { Name = "IsCleaning", };
            if (record.Value == "1")
                return BadRequest("Unable to clean right now, another process is cleaning the database");

            var allCats = await _categoryService.GetRelatedCategories(CategoryId);
            if (allCats.Count == 0)
                return BadRequest("Category Id is empty");

            record.Value = "1";
            if (record.Id == 0) _settingsService.Insert(record);
            else _settingsService.Update(record);
            await _unitOfWork.SaveChangesAsync();

            BackgroundJob.Enqueue<SettingsService>(x => x.CleanDb(allCats));
            return Ok(true);
        }

        [HttpPost("CleanDatabase/{CategoryId}/{BrandId}")]
        public async Task<ActionResult<bool>> CleanDatabaseWithBrand(int CategoryId = 0, int BrandId = 0)
        {
            var record = _settingsService.Queryable().FirstOrDefault(x => x.Name == "IsCleaning") ?? new Settings { Name = "IsCleaning", };
            if (record.Value == "1")
                return BadRequest("Unable to clean right now, another process is cleaning the database");

            var allCats = await _categoryService.GetRelatedCategories(CategoryId);
            if (allCats.Count == 0)
                return BadRequest("Category Id is empty");

            record.Value = "1";
            if (record.Id == 0) _settingsService.Insert(record);
            else _settingsService.Update(record);
            await _unitOfWork.SaveChangesAsync();

            BackgroundJob.Enqueue<SettingsService>(x => x.CleanDbWithBrandId(allCats, BrandId));
            return Ok(true);
        }
    }
}
