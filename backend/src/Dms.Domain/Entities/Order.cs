using Dms.Domain.Common;

namespace Dms.Domain.Entities;

public sealed class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public Guid DistributorId { get; set; }
    public Distributor Distributor { get; set; } = null!;
    public DateOnly OrderDate { get; set; }
    public string Status { get; set; } = "Draft";
    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<OrderApproval> Approvals { get; set; } = new List<OrderApproval>();
}

public sealed class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal LineGross { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal LineNet { get; set; }
    public decimal FreeQuantity { get; set; }
    public decimal CashbackAmount { get; set; }
    public decimal Points { get; set; }
}

public sealed class OrderApproval : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int LevelNo { get; set; }
    public string ApproverRole { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public Guid? ActionByUserId { get; set; }
    public DateTime? ActionAtUtc { get; set; }
    public string? Remarks { get; set; }
}

public sealed class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid DistributorId { get; set; }
    public Distributor Distributor { get; set; } = null!;
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public DateOnly InvoiceDate { get; set; }
    public decimal Amount { get; set; }
    public string SyncStatus { get; set; } = "Pending";
}
