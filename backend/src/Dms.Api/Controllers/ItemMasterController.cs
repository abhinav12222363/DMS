using Dms.Application.Masters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dms.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin,Distributor")]
[Route("api/master/items")]
public sealed class ItemMasterController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemMasterController(IItemService itemService) => _itemService = itemService;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await _itemService.GetAsync(search, pageNumber, Math.Clamp(pageSize, 1, 100), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertItemRequest request, CancellationToken ct)
        => Ok(await _itemService.CreateAsync(request, ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertItemRequest request, CancellationToken ct)
        => Ok(await _itemService.UpdateAsync(id, request, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _itemService.DeleteAsync(id, ct);
        return NoContent();
    }
}
