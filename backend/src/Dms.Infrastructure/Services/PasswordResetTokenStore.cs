using Dms.Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace Dms.Infrastructure.Services;

public sealed class PasswordResetTokenStore : IPasswordResetTokenStore
{
    private readonly IDistributedCache _distributedCache;

    public PasswordResetTokenStore(IDistributedCache distributedCache) => _distributedCache = distributedCache;

    public Task StoreAsync(Guid userId, string token, CancellationToken ct)
    {
        return _distributedCache.SetStringAsync($"pwd-reset:{token}", userId.ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20)
        }, ct);
    }

    public async Task<Guid?> ConsumeAsync(string token, CancellationToken ct)
    {
        var key = $"pwd-reset:{token}";
        var value = await _distributedCache.GetStringAsync(key, ct);
        if (value is null || !Guid.TryParse(value, out var userId))
        {
            return null;
        }

        await _distributedCache.RemoveAsync(key, ct);
        return userId;
    }
}
