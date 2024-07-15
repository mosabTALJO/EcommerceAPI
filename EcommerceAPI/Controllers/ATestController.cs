using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Web.Controllers
{
    public class ATestController : MyControllerBase
    {
        private readonly MyDbContext myDbContext;
        public ATestController(MyDbContext myDbContext)
        {
            this.myDbContext = myDbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Test()
        {
            try
            {
                var search = "test Product";
                var s1 = myDbContext.Products.Where(x => EF.Functions.Like(x.NameTr, search + "%")).ToQueryString();
                var s2 = myDbContext.Products.Where(x => x.NameTr!.Contains(search)).ToQueryString();
                //var s3 = myDbContext.Products.Where(x => x.NameTr!.Contains(search, StringComparison.OrdinalIgnoreCase)).ToQueryString();
                var s4 = myDbContext.Products.Where(x => EF.Functions.Like(x.NameTr!.ToLower(), $"{search}%".ToLower())).ToQueryString();
                //var s5 = myDbContext.Products.Where(x => EFHelper.LikeIgnoreCase(x.NameTr!, $"{search}%")).ToQueryString();
                //var s6 = myDbContext.Products.Where(x => EF.Functions.LikeIgnoreCase(x.NameTr!, $"{search}%")).ToQueryString();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + "\n---------------------" + ex.InnerException?.Message);
            }
        }
    }
}
