using CQRSyMediatR.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CQRSyMediatR.Infrastructure.Persistence;
public class MyAppDbContext : IdentityDbContext<IdentityUser>
{
    public MyAppDbContext(DbContextOptions<MyAppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Product> Products => Set<Product>();
}