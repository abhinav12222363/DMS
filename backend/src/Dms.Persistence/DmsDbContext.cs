using Dms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dms.Persistence;

public sealed class DmsDbContext : DbContext
{
    public DmsDbContext(DbContextOptions<DmsDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Distributor> Distributors => Set<Distributor>();
    public DbSet<UserDistributor> UserDistributors => Set<UserDistributor>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DmsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
