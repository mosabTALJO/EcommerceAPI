using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services
{
    public interface IBrandService : IService<Brand>
    {
        bool CanDelete(int brandId);
        bool IsValid(Brand b, out string error);
        int[] GetBrandIdsByCategoryId(int categoryId);
    }

    public class BrandService : Service<Brand>, IBrandService
    {
        IRepository<Product> _pRepo;
        IRepository<Category> _cRepo;
        public BrandService(IRepository<Brand> repo, IRepository<Category> cRepo, IRepository<Product> pRepo) : base(repo)
        {
            _pRepo = pRepo;
            _cRepo = cRepo;
        }

        public bool CanDelete(int brandId) =>
            !_pRepo.Queryable().Any(x => x.BrandId == brandId);
        public int[] GetBrandIdsByCategoryId(int categoryId)
        {
            var searcCategories = new List<int> { categoryId };
            var allCats = _cRepo.Queryable().Where(x => x.ParentId == categoryId).Include(x => x.Children).ToArray();
            searcCategories.AddRange(allCats.Select(x => x.Id));
            searcCategories.AddRange(allCats.SelectMany(x => x.Children.Select(x => x.Id)));

            var productIds = _pRepo.Queryable().Where(x => x.CategoryId != null && searcCategories.Contains(x.CategoryId.Value)).Select(x => x.Id).Distinct();
            return _pRepo.Queryable().Where(x => x.BrandId.HasValue && productIds.Contains(x.Id)).Select(x => x.BrandId.Value).Distinct().ToArray();
        }
        public bool IsValid(Brand b, out string error)
        {
            var errors = new List<string>();
            //if (string.IsNullOrWhiteSpace(b.NameTr))
            //    errors.Add("NameTr is Not valid");
            if (string.IsNullOrWhiteSpace(b.NameEn))
                errors.Add("NameEn is Not valid");
            if (string.IsNullOrWhiteSpace(b.NameAr))
                errors.Add("NameAr is Not valid");

            error = string.Join("\n", errors);
            return string.IsNullOrEmpty(error);
        }
    }
}