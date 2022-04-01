using MediatR;
using CQRSyMediatR.Domain;
using CQRSyMediatR.Infrastructure.Persistence;

using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
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