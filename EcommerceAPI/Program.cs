using EcommerceAPI.Core;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
        builder.WithOrigins("*", "http://localhost:4200", "http://localhost:5173")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});
builder.Services.AddAuthorization();
builder.Services.AddDbContext<MyDbContext>(x => 
x.UseNpgsql("Host=localhost; Database=Ecommerce; Username=ecomm; Password=aser@sWwer##4as"));
builder.Services.AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<MyDbContext>();

var app = builder.Build();
using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetService<MyDbContext>();
    dbContext?.Database.Migrate();
}

//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapIdentityApi<AppUser>();
app.UseHttpsRedirection();
app.UseAuthorization();
//app.UseMiddleware<ErrorHandlerMiddleware>();
app.MapControllers();

app.Run();