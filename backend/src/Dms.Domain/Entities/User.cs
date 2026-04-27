using Dms.Domain.Common;
using Dms.Domain.Enums;

namespace Dms.Domain.Entities;

public sealed class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<UserDistributor> UserDistributors { get; set; } = new List<UserDistributor>();
}
