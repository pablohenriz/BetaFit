using BetaFit.Domain.Entities;
using BetaFit.Domain.Interfaces;
using BetaFit.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BetaFit.Infrastructure.Context;

/// <summary>
/// Único ponto de acesso ao banco de dados no projeto.
/// Nem o Website (BetaFit.UI) nem o Desktop (BetaFit.Desktop) conhecem esta classe -
/// eles falam apenas com a BetaFit.API através de HTTP.
/// </summary>
public class BetaFitDbContext : DbContext, IUnitOfWork
{
    public BetaFitDbContext(DbContextOptions<BetaFitDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());

        modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);

        base.OnModelCreating(modelBuilder);
    }
}
