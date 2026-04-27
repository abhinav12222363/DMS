using Dms.Domain.Common;

namespace Dms.Domain.Entities;

public sealed class Stock : BaseEntity
{
    public Guid DistributorId { get; set; }
    public Distributor Distributor { get; set; } = null!;
    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal ReorderLevel { get; set; }
}

public sealed class Scheme : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string SchemeType { get; set; } = "OnSpot";
    public string ApplyOn { get; set; } = "Item";
    public string CalculationMode { get; set; } = "Exclusive";
    public string Status { get; set; } = "Draft";
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidTo { get; set; }
    public Guid? ItemId { get; set; }
    public string? ItemGroup { get; set; }
    public string? Attribute { get; set; }
    public ICollection<SchemeSlab> Slabs { get; set; } = new List<SchemeSlab>();
}

public sealed class SchemeSlab : BaseEntity
{
    public Guid SchemeId { get; set; }
    public Scheme Scheme { get; set; } = null!;
    public decimal MinQty { get; set; }
    public decimal? MaxQty { get; set; }
    public decimal MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal FreeQty { get; set; }
    public decimal CashbackAmount { get; set; }
    public decimal Points { get; set; }
}

public sealed class Claim : BaseEntity
{
    public Guid DistributorId { get; set; }
    public Distributor Distributor { get; set; } = null!;
    public string ClaimType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending";
    public string Reason { get; set; } = string.Empty;
    public ICollection<ClaimDocument> Documents { get; set; } = new List<ClaimDocument>();
}

public sealed class ClaimDocument : BaseEntity
{
    public Guid ClaimId { get; set; }
    public Claim Claim { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
}

public sealed class Replenishment : BaseEntity
{
    public Guid DistributorId { get; set; }
    public Distributor Distributor { get; set; } = null!;
    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public decimal CurrentStock { get; set; }
    public decimal SuggestedQty { get; set; }
    public string Status { get; set; } = "Suggested";
}
