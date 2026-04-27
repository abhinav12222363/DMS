using Dms.Application.Common;

namespace Dms.Application.Masters;

public sealed record ItemDto(Guid Id, string ItemCode, string Name, string Unit, string Group);
public sealed record UpsertItemRequest(string ItemCode, string Name, string Unit, string Group);

public interface IItemService
{
    Task<PagedResult<ItemDto>> GetAsync(string? search, int pageNumber, int pageSize, CancellationToken ct);
    Task<ItemDto> CreateAsync(UpsertItemRequest request, CancellationToken ct);
    Task<ItemDto> UpdateAsync(Guid id, UpsertItemRequest request, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}
