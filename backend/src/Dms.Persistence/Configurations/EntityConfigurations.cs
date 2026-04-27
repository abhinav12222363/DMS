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
        builder.Property(x => x.CreditLimit).HasPrecision(18, 2);
        builder.Property(x => x.OutstandingAmount).HasPrecision(18, 2);
    }
}

public sealed class UserDistributorConfiguration : IEntityTypeConfiguration<UserDistributor>
{
    public void Configure(EntityTypeBuilder<UserDistributor> builder) => builder.HasKey(x => new { x.UserId, x.DistributorId });
}

public sealed class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasIndex(x => x.ItemCode).IsUnique();
        builder.HasIndex(x => x.Name);
        builder.Property(x => x.Moq).HasPrecision(18, 2);
        builder.Property(x => x.BasePrice).HasPrecision(18, 2);
    }
}

public sealed class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.HasIndex(x => new { x.DistributorId, x.OrderDate });
        builder.HasIndex(x => x.OrderNumber).IsUnique();
    }
}

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.HasIndex(x => x.OrderNumber).IsUnique();
        builder.Property(x => x.GrossAmount).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.NetAmount).HasPrecision(18, 2);
    }
}

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");
        builder.Property(x => x.Quantity).HasPrecision(18, 2);
        builder.Property(x => x.Rate).HasPrecision(18, 2);
        builder.Property(x => x.LineGross).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.LineNet).HasPrecision(18, 2);
        builder.Property(x => x.FreeQuantity).HasPrecision(18, 2);
        builder.Property(x => x.CashbackAmount).HasPrecision(18, 2);
        builder.Property(x => x.Points).HasPrecision(18, 2);
    }
}

public sealed class OrderApprovalConfiguration : IEntityTypeConfiguration<OrderApproval>
{
    public void Configure(EntityTypeBuilder<OrderApproval> builder)
    {
        builder.ToTable("order_approvals");
        builder.HasIndex(x => new { x.OrderId, x.LevelNo }).IsUnique();
    }
}

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoices");
        builder.HasIndex(x => x.InvoiceNumber).IsUnique();
        builder.Property(x => x.Amount).HasPrecision(18, 2);
    }
}

public sealed class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.ToTable("stock");
        builder.HasIndex(x => new { x.DistributorId, x.ItemId }).IsUnique();
        builder.Property(x => x.Quantity).HasPrecision(18, 2);
        builder.Property(x => x.ReorderLevel).HasPrecision(18, 2);
    }
}

public sealed class SchemeConfiguration : IEntityTypeConfiguration<Scheme>
{
    public void Configure(EntityTypeBuilder<Scheme> builder)
    {
        builder.ToTable("schemes");
        builder.Property(x => x.Name).HasMaxLength(120);
    }
}

public sealed class SchemeSlabConfiguration : IEntityTypeConfiguration<SchemeSlab>
{
    public void Configure(EntityTypeBuilder<SchemeSlab> builder)
    {
        builder.ToTable("scheme_slabs");
        builder.Property(x => x.MinQty).HasPrecision(18, 2);
        builder.Property(x => x.MinValue).HasPrecision(18, 2);
        builder.Property(x => x.DiscountPercent).HasPrecision(18, 2);
        builder.Property(x => x.FreeQty).HasPrecision(18, 2);
        builder.Property(x => x.CashbackAmount).HasPrecision(18, 2);
        builder.Property(x => x.Points).HasPrecision(18, 2);
    }
}

public sealed class ClaimConfiguration : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        builder.ToTable("claims");
        builder.Property(x => x.Amount).HasPrecision(18, 2);
    }
}

public sealed class ClaimDocumentConfiguration : IEntityTypeConfiguration<ClaimDocument>
{
    public void Configure(EntityTypeBuilder<ClaimDocument> builder) => builder.ToTable("claim_documents");
}

public sealed class ReplenishmentConfiguration : IEntityTypeConfiguration<Replenishment>
{
    public void Configure(EntityTypeBuilder<Replenishment> builder)
    {
        builder.ToTable("replenishment");
        builder.Property(x => x.CurrentStock).HasPrecision(18, 2);
        builder.Property(x => x.SuggestedQty).HasPrecision(18, 2);
    }
}
