using CQRSyMediatR.Domain;
using Microsoft.EntityFrameworkCore;

namespace CQRSyMediatR.Infrastructure.Persistence;
public class MyAppDbContext : DbContext
{
    public MyAppDbContext(DbContextOptions<MyAppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Product> Products => Set<Product>();
}