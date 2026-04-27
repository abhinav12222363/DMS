using Dms.Domain.Common;

namespace Dms.Domain.Entities;

public sealed class Distributor : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<UserDistributor> UserDistributors { get; set; } = new List<UserDistributor>();
}
