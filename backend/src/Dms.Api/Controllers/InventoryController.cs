using Dms.Application.Workflows;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dms.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/inventory")]
public sealed class InventoryController : ControllerBase
{
    private readonly IInventoryService _service;
    public InventoryController(IInventoryService service) => _service = service;

    [HttpGet("stock")]
    public async Task<IActionResult> Get([FromQuery] Guid distributorId, CancellationToken ct)
        => Ok(await _service.GetStockAsync(distributorId, ct));

    [HttpPost("stock-adjustment")]
    public async Task<IActionResult> Adjust([FromBody] StockAdjustmentRequest request, CancellationToken ct)
        => Ok(await _service.AdjustAsync(request, ct));
}
