using Dms.Domain.Common;

namespace Dms.Domain.Entities;

public sealed class SalesOrder : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public Guid DistributorId { get; set; }
    public Distributor Distributor { get; set; } = null!;
    public DateOnly OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Draft";
}
