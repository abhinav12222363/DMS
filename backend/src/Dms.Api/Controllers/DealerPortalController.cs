using Dms.Application.Workflows;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dms.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/dealer/orders")]
public sealed class DealerPortalController : ControllerBase
{
    private readonly IDealerPortalService _service;
    public DealerPortalController(IDealerPortalService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
        => Ok(await _service.CreateOrderAsync(request, ct));

    [HttpPost("{orderId:guid}/copy")]
    public async Task<IActionResult> Copy(Guid orderId, [FromQuery] DateOnly newDate, CancellationToken ct)
        => Ok(await _service.CopyOrderAsync(orderId, newDate, ct));

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid distributorId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await _service.GetOrdersAsync(distributorId, pageNumber, Math.Clamp(pageSize, 1, 100), ct));
}
