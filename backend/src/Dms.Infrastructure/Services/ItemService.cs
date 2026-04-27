using Dms.Application.Abstractions;
using Dms.Application.Common;
using Dms.Application.Masters;
using Dms.Domain.Entities;

namespace Dms.Infrastructure.Services;

public sealed class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;

    public ItemService(IItemRepository itemRepository) => _itemRepository = itemRepository;

    public async Task<PagedResult<ItemDto>> GetAsync(string? search, int pageNumber, int pageSize, CancellationToken ct)
    {
        var paged = await _itemRepository.GetPagedAsync(search, pageNumber, pageSize, ct);
        return new PagedResult<ItemDto>(
            paged.Items.Select(x => new ItemDto(x.Id, x.ItemCode, x.Name, x.Unit, x.Group)).ToArray(),
            paged.PageNumber,
            paged.PageSize,
            paged.TotalCount);
    }

    public async Task<ItemDto> CreateAsync(UpsertItemRequest request, CancellationToken ct)
    {
        var entity = new Item { ItemCode = request.ItemCode, Name = request.Name, Unit = request.Unit, Group = request.Group };
        await _itemRepository.AddAsync(entity, ct);
        await _itemRepository.SaveChangesAsync(ct);
        return new ItemDto(entity.Id, entity.ItemCode, entity.Name, entity.Unit, entity.Group);
    }

    public async Task<ItemDto> UpdateAsync(Guid id, UpsertItemRequest request, CancellationToken ct)
    {
        var entity = await _itemRepository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Item not found.");
        entity.ItemCode = request.ItemCode;
        entity.Name = request.Name;
        entity.Unit = request.Unit;
        entity.Group = request.Group;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        await _itemRepository.SaveChangesAsync(ct);
        return new ItemDto(entity.Id, entity.ItemCode, entity.Name, entity.Unit, entity.Group);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await _itemRepository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Item not found.");
        await _itemRepository.RemoveAsync(entity, ct);
        await _itemRepository.SaveChangesAsync(ct);
    }
}
