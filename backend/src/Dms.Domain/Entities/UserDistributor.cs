namespace Dms.Domain.Entities;

public sealed class UserDistributor
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid DistributorId { get; set; }
    public Distributor Distributor { get; set; } = null!;
}
