using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Web.Controllers
{
    [ApiExplorerSettings(GroupName = "cp")]
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
        [Authorize(Policy = PoliciesText.CleaningDatabase)]
        public ActionResult<bool> CanCleanDatabase() =>
           Ok((_settingsService.Queryable().FirstOrDefault(x => x.Name == "IsCleaning")?.Value ?? "0") != "1");

        [HttpPost("CleanDatabase/{CategoryId}")]
        [Authorize(Policy = PoliciesText.CleaningDatabase)]
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
        [Authorize(Policy = PoliciesText.CleaningDatabase)]
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

        [HttpPost("UpdateAppSettings")]
        [Authorize(Policy = PoliciesText.AdminPermissions)]
        public async Task<ActionResult<RequestResult<bool>>> UpdateSettings([FromBody] Settings[] settings)
        {
            var oldSettings = settings.Where(x => x.Id > 0).ToArray();
            var oldSettingsIds = oldSettings.Select(x => x.Id).ToArray();

            var dbSettings = _settingsService.Queryable().Where(x => oldSettingsIds.Contains(x.Id)).ToArray();
            foreach (var dbSetting in dbSettings)
            {
                var s = settings.First(x => x.Id == dbSetting.Id);
                dbSetting.Name = s.Name;
                dbSetting.Value = s.Value;
            }
            var newSettings = settings.Except(oldSettings).ToList();
            newSettings.ForEach(x => _settingsService.Insert(x));
            await _unitOfWork.SaveChangesAsync();
            return new RequestResult<bool>(true, ActionResultType.OK);
        }

        [HttpGet("GetAppSettings")]
        [AllowAnonymous]
        public async Task<ActionResult<RequestResult<Settings[]>>> GetAppSettings()
        {
            var dbSettings = await _settingsService.Queryable().Where(x => !x.IsAdmin).ToArrayAsync();
            return new RequestResult<Settings[]>(dbSettings, ActionResultType.OK);
        }

        [HttpGet("GetAdminSettings")]
        [AllowAnonymous]
        public async Task<ActionResult<RequestResult<Settings[]>>> GetAdminSettings()
        {
            var dbSettings = await _settingsService.Queryable().Where(x => x.IsAdmin).ToArrayAsync();
            return new RequestResult<Settings[]>(dbSettings, ActionResultType.OK);
        }
    }
}
