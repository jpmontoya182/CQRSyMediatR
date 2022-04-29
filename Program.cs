using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using CQRSyMediatR.Behaviours;
using CQRSyMediatR.Domain;
using CQRSyMediatR.Filters;
using CQRSyMediatR.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using CQRSyMediatR.Services;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
   {
     new OpenApiSecurityScheme
     {
       Reference = new OpenApiReference
       {
         Type = ReferenceType.SecurityScheme,
         Id = "Bearer"
       }
      },
      new string[] { }
    }
  });
});

builder.Services.AddControllers(options => options.Filters.Add<ApiExceptionFilterAttribute>());
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
builder.Services.AddSqlite<MyAppDbContext>(builder.Configuration.GetConnectionString("Default"));// 
// adding automapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
// Identity Core
builder.Services
    .AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<MyAppDbContext>();

// Autenticación y autorización
builder.Services
    .AddHttpContextAccessor()
    .AddAuthorization()
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await SeedDataBase();
app.Run();

// seed of data in products table
async Task SeedDataBase()
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MyAppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    // products
    if (!context.Products.Any())
    {
        context.Products.AddRange(new List<Product>
        {
            new Product { Description = "Product 01", Price = 16000 },
            new Product { Description = "Product 02", Price = 52200 }
        });
        await context.SaveChangesAsync();
    }
    // users
    var testUser = await userManager.FindByNameAsync("test_user");
    if (testUser is null)
    {
        await userManager.CreateAsync(new IdentityUser
        {
            UserName = "test_user"
        }, "Passw0rd.1234");

        await userManager.CreateAsync(new IdentityUser
        {
            UserName = "other_user"
        }, "Passw0rd.1234");
    }
    // admins
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var adminRole = await roleManager.FindByNameAsync("Admin");
    if (adminRole is null)
    {
        await roleManager.CreateAsync(new IdentityRole
        {
            Name = "Admin"
        });
        await userManager.AddToRoleAsync(testUser, "Admin");
    }
}

