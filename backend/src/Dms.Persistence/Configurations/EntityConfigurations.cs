using Dms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dms.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(x => x.Username).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.Username).HasMaxLength(80);
        builder.Property(x => x.Email).HasMaxLength(256);
    }
}

public sealed class DistributorConfiguration : IEntityTypeConfiguration<Distributor>
{
    public void Configure(EntityTypeBuilder<Distributor> builder)
    {
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Zone).HasMaxLength(40);
    }
}

public sealed class UserDistributorConfiguration : IEntityTypeConfiguration<UserDistributor>
{
    public void Configure(EntityTypeBuilder<UserDistributor> builder)
    {
        builder.HasKey(x => new { x.UserId, x.DistributorId });
    }
}

public sealed class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasIndex(x => x.ItemCode).IsUnique();
        builder.HasIndex(x => x.Name);
    }
}

public sealed class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.HasIndex(x => new { x.DistributorId, x.OrderDate });
        builder.HasIndex(x => x.OrderNumber).IsUnique();

        // PostgreSQL table partitioning strategy (monthly range partition by OrderDate):
        // 1. create parent table via migration SQL with PARTITION BY RANGE ("OrderDate")
        // 2. create partitions: sales_orders_2026_04 FOR VALUES FROM ('2026-04-01') TO ('2026-05-01')
        // 3. automate partition creation using pg_partman or scheduled job.
    }
}
