using EcommerceAPI.Data.Helpers;
using EcommerceAPI.Data.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EcommerceAPI.Core
{
    public class MyDbContext : IdentityDbContext<AppUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MyDbContext(DbContextOptions<MyDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseNpgsql(@"Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase");
        //}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //To Add Index
            //modelBuilder.Entity<Notification>().HasIndex(c => c.UserId);

            modelBuilder.Entity<Category>().HasIndex(c => c.NameTr);
            modelBuilder.Entity<Category>().HasIndex(c => c.CompanyUrl);
            modelBuilder.Entity<Category>().HasIndex(p => p.IsActive);
            modelBuilder.Entity<Category>().HasIndex(p => p.OrderIndex);
            modelBuilder.Entity<Category>().HasQueryFilter(p => p.IsActive);

            modelBuilder.Entity<Brand>().HasIndex(c => c.CompanyUrl);
            modelBuilder.Entity<Brand>().HasIndex(c => c.NameTr);
            modelBuilder.Entity<Brand>().HasIndex(c => c.NameAr);
            modelBuilder.Entity<Brand>().HasIndex(c => c.NameEn);
            modelBuilder.Entity<Brand>().HasIndex(p => p.OrderIndex);
            modelBuilder.Entity<Brand>().HasIndex(p => p.IsActive);
            modelBuilder.Entity<Brand>().HasQueryFilter(p => p.IsActive);

            modelBuilder.Entity<Product>().HasIndex(p => p.CompanyProductID);
            modelBuilder.Entity<Product>().HasIndex(p => p.NameEn);
            modelBuilder.Entity<Product>().HasIndex(p => p.NameAr);
            modelBuilder.Entity<Product>().HasIndex(p => p.NameTr);
            modelBuilder.Entity<Product>().HasIndex(p => p.SKU);
            modelBuilder.Entity<Product>().HasIndex(p => p.CreatedAt);
            modelBuilder.Entity<Product>().HasIndex(p => p.CompanyUpdatedDate);
            modelBuilder.Entity<Product>().HasIndex(p => p.IsActive);
            modelBuilder.Entity<Product>().HasIndex(p => p.CompanyBrand);
            modelBuilder.Entity<Product>().HasQueryFilter(p => p.IsActive);
            modelBuilder.Entity<Product>().HasIndex(p => p.DeletedAt);
            modelBuilder.Entity<Product>().HasIndex(p => p.CategoryId);

            modelBuilder.Entity<OrderCategoryBrand>().HasIndex(p => p.CategoryId);
            modelBuilder.Entity<OrderCategoryBrand>().HasIndex(p => p.BrandId);
            modelBuilder.Entity<OrderCategoryBrand>().HasIndex(p => p.OrderIndex);

            base.OnModelCreating(modelBuilder);
        }
        void UpdateAuditableFields()
        {
            var entries = ChangeTracker.Entries();
            var now = DateTime.UtcNow;
            var user = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "Anonymous";

            foreach (var entry in entries)
            {
                if (entry.Entity is IMyTrackable entity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedBy = user;
                        entity.CreatedAt = now;
                        entity.UpdatedBy = user;
                        entity.UpdatedAt = now;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        if (entity.UpdateCreatedAt)
                            entity.CreatedAt = now;
                        entity.UpdatedBy = user;
                        entity.UpdatedAt = now;
                    }
                    else if (entry.State == EntityState.Deleted && entry.Entity is ISoftDelete sd)
                    {
                        if (!sd.CompleteRemove)
                        {
                            entry.State = EntityState.Modified;
                            entity.UpdatedBy = user;
                            entity.UpdatedAt = now;
                            sd.DeletedBy = user;
                            sd.DeletedAt = now;
                        }
                    }
                }
            }
        }
        public override int SaveChanges()
        {
            //Update Auditable Fields
            UpdateAuditableFields();
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            //Update Auditable Fields
            UpdateAuditableFields();
            return base.SaveChangesAsync(cancellationToken);
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<ProductFavorite> ProductFavorites { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CurrencyRate> CurrencyRates { get; set; }
        public DbSet<OrderCategoryBrand> OrderCategoryBrands { get; set; }
        public DbSet<CollectionItem> CollectionItems { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<OrderLog> OrderLogs { get; set; }
        public DbSet<OrderPayment> OrderPayments { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
    }
}