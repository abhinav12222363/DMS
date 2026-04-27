using Dms.Domain.Common;

namespace Dms.Domain.Entities;

public sealed class Item : BaseEntity
{
    public string ItemCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
}
