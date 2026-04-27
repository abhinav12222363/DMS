using Dms.Application.Abstractions;
using Dms.Application.Distributors;

namespace Dms.Infrastructure.Services;

public interface IDistributorService
{
    Task<IReadOnlyCollection<DistributorDto>> GetForUserAsync(Guid userId, CancellationToken ct);
}

public sealed class DistributorService : IDistributorService
{
    private readonly IDistributorRepository _distributorRepository;

    public DistributorService(IDistributorRepository distributorRepository) => _distributorRepository = distributorRepository;

    public Task<IReadOnlyCollection<DistributorDto>> GetForUserAsync(Guid userId, CancellationToken ct) =>
        _distributorRepository.GetForUserAsync(userId, ct);
}
