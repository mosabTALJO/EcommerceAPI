using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Seeds;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.Helpers.Abstractions;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.EF;
using EcommerceAPI.Web.Helpers;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text;

TokenValidationParameters tokenValidationParameters;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<DbContext, MyDbContext>();

foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
{
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assembly));
    builder.Services.AddAutoMapper(assembly);
}
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IUserAddressService, UserAddressService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IProductFavoriteService, ProductFavoriteService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IOrderCategoryBrandService, OrderCategoryBrandService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<ICollectionService, CollectionService>();
builder.Services.AddScoped<IEmailPromotionService, EmailPromotionService>();
builder.Services.AddScoped<IBaseItemService, BaseItemService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IFrontendSettingsService, FrontendSettingsService>();
builder.Services.AddScoped<IOrderLogService, OrderLogService>();
builder.Services.AddScoped<IOrderPaymentService, OrderPaymentService>();
builder.Services.AddScoped<IAttributesService, AttributesService>();
builder.Services.AddScoped<IAppEmailSender, SendGridEmailSender>();
builder.Services.AddScoped<IProductVariantService, ProductVariantService>();
builder.Services.AddScoped<IKargoCompanyService, KargoCompanyService>();
builder.Services.AddTransient<ICurrencyRateService, CurrencyRateService>();

//////////////////////////////

tokenValidationParameters = new TokenValidationParameters()
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidAudience = builder.Configuration["Jwt:Site"],
    ValidIssuer = builder.Configuration["Jwt:Site"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"]))
};

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
   .AddEntityFrameworkStores<MyDbContext>()
   .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2));

builder.Services.AddAuthorization(option =>
{
    foreach (Policies p in Enum.GetValues(typeof(Policies)))
        option.AddPolicy(p.GetPolicyNumber(), policy => policy.AddRequirements(new AdminHasAccessRequirement(p.GetPolicyNumber())));
});
builder.Services.AddScoped<IAuthorizationHandler, AdminHasAccessHandler>();

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = tokenValidationParameters;
});

//////////////////////////////
builder.Services.Configure<SendGridEmailSenderOptions>(options =>
{
    options.ApiKey = builder.Configuration["ExternalProviders:SendGrid:ApiKey"];
    options.SenderEmail = builder.Configuration["ExternalProviders:SendGrid:SenderEmail"];
    options.SenderName = builder.Configuration["ExternalProviders:SendGrid:SenderName"];
});
builder.Services.AddHangfire(x =>
{
    x.UseSqlServerStorage("server=localhost;database=EcommerceHangfire;User ID=EcommerceHF;password=xvu3Ki5121;MultipleActiveResultSets=True;Encrypt=False");
    x.UseFilter(new AutomaticRetryAttribute { Attempts = 0 });
});
builder.Services.AddHangfireServer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("cp", new OpenApiInfo { Title = "Control Panel", Version = "cp" });
    c.SwaggerDoc("fe", new OpenApiInfo { Title = "Mobile", Version = "fe" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers(options => options.OutputFormatters.RemoveType<StringOutputFormatter>());

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
    builder =>
    {
        builder.WithOrigins("*", 
            "http://localhost:4200",
            "http://localhost:5173",
            "https://cp-ecommerce.usos.co", 
            "https://Ecommerce.usos.co")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});
ConfigureLocalization(builder.Services);
builder.Services.AddDbContext<MyDbContext>(x => x.UseNpgsql("Host=localhost; Database=Ecommerce; Username=ecomm; Password=aser@sWwer##4as"));

var app = builder.Build();
if (!builder.Environment.IsDevelopment())
{
    using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        var dbContext = serviceScope.ServiceProvider.GetService<MyDbContext>();
        var userManager = serviceScope.ServiceProvider.GetService<UserManager<AppUser>>();
        var memoryCache = serviceScope.ServiceProvider.GetService<IMemoryCache>();
        var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
        var recurringJobManager = serviceScope.ServiceProvider.GetService<IRecurringJobManager>();
        dbContext?.Database.Migrate();

        MySeedInitializer.SeedData(userManager, roleManager, memoryCache);
        var options = serviceScope.ServiceProvider.GetService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(options.Value);

        if (!builder.Environment.IsDevelopment())
        {
            recurringJobManager.RemoveIfExists("RefreshPrices");
            recurringJobManager.AddOrUpdate<CurrencyRateService>("RefreshPrices", x => x.RefreshRates(), Cron.Hourly(1));
        }
    }
}

//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/cp/swagger.json", "Control Panel");
        c.SwaggerEndpoint("/swagger/fe/swagger.json", "Mobile");
    });
}
app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = [new MyAuthorizationFilter(tokenValidationParameters)] });

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.MapControllers();




app.Run();



void ConfigureLocalization(IServiceCollection services)
{
    services.AddLocalization(options => options.ResourcesPath = "Resources");
    services.Configure<RequestLocalizationOptions>(options =>
    {
        List<CultureInfo> supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                    new CultureInfo("tr"),
                    new CultureInfo("ar"),
                    new CultureInfo("fr"),
                };
        options.DefaultRequestCulture = new RequestCulture("en");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;
        options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new AcceptLanguageHeaderRequestCultureProvider(),
                    new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider()
                };
    });
}