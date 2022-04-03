using MediatR;
using CQRSyMediatR.Domain;
using CQRSyMediatR.Infrastructure.Persistence;

using System.Reflection;
using CQRSyMediatR.Filters;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using CQRSyMediatR.Behaviours;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(options => options.Filters.Add<ApiExceptionFilterAttribute>());
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddSqlite<MyAppDbContext>(builder.Configuration.GetConnectionString("Default"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

await SeedProducts();
app.Run();

// seed of data in products table
async Task SeedProducts()
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MyAppDbContext>();

    if (!context.Products.Any())
    {
        context.Products.AddRange(new List<Product>
        {
            new Product { Description = "Product 01", Price = 16000 },
            new Product { Description = "Product 02", Price = 52200 }
        });
        await context.SaveChangesAsync();
    }
}