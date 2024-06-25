using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.EF;
using EcommerceAPI.UnitOfWork.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceAPI.Services
{
    public interface ICategoryService : IService<Category>
    {
        Task<List<Category>> GetLiteCategories();
        bool CanDelete(int id);
        Task<List<int>> GetRelatedCategories(int categoryId);
    }

    public class CategoryService : Service<Category>, ICategoryService
    {
        IRepository<Product> _pRepo;
        readonly ILogger<CategoryService> _logger;
        public CategoryService(IRepository<Category> repo, IRepository<Product> pRepo, ILogger<CategoryService> logger) : base(repo)
        {
            _pRepo = pRepo;
            _logger = logger;
        }

        public bool CanDelete(int id) =>
            !_pRepo.Queryable().Any(x => x.CategoryId == id) && !Repository.Queryable().Any(x => x.ParentId == id);

        public async Task<List<Category>> GetLiteCategories()
        {
            var cats = await Repository.Queryable()
                        .Where(x => x.ParentId == null)
                        .Include(x => x.Children).ThenInclude(x => x.Children)//.ThenInclude(x => x.Children).ThenInclude(x => x.Children).ThenInclude(x => x.Children)
                        .ToListAsync();
            var all = new List<Category>();
            foreach (var c in cats)
            {
                string tempParent1 = c.NameEn;
                c.NameEn = c.Id + ": " + c.NameEn;
                all.Add(c);
                foreach (var c1 in c.Children ?? [])
                {
                    string tempParent2 = tempParent1 + " -> " + c1.NameEn;
                    c1.NameEn = c1.Id + ": " + tempParent1 + " -> " + c1.NameEn;
                    all.Add(c1);
                    foreach (var c2 in c1.Children ?? [])
                    {
                        c2.NameEn = c2.Id + ": " + tempParent2 + " -> " + c2.NameEn;
                        all.Add(c2);
                    }
                }
            }
            return all;
        }

        public async Task<List<int>> GetRelatedCategories(int categoryId)
        {
            List<int> searchCategories = [categoryId];

            var childrenIds = await Repository.Queryable().Where(x => x.ParentId.HasValue && x.ParentId == categoryId && x.IsActive).Select(x => x.Id).ToArrayAsync();
            if (childrenIds.Length > 0)
            {
                searchCategories.AddRange(childrenIds);
                childrenIds = await Repository.Queryable().Where(x => x.ParentId.HasValue && childrenIds.Contains(x.ParentId.Value) && x.IsActive).Select(x => x.Id).ToArrayAsync();
                if (childrenIds.Length > 0)
                    searchCategories.AddRange(childrenIds);
            }

            return searchCategories;
        }
    }
}