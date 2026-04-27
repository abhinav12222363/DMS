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

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderApproval> OrderApprovals => Set<OrderApproval>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<Scheme> Schemes => Set<Scheme>();
    public DbSet<SchemeSlab> SchemeSlabs => Set<SchemeSlab>();
    public DbSet<Claim> Claims => Set<Claim>();
    public DbSet<ClaimDocument> ClaimDocuments => Set<ClaimDocument>();
    public DbSet<Replenishment> Replenishments => Set<Replenishment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DmsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
