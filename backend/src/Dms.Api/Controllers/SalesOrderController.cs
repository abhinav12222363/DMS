using Dms.Application.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dms.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/transactions/sales-orders")]
public sealed class SalesOrderController : ControllerBase
{
    private readonly ISalesOrderService _service;

    public SalesOrderController(ISalesOrderService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid distributorId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await _service.GetAsync(distributorId, pageNumber, Math.Clamp(pageSize, 1, 100), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSalesOrderRequest request, CancellationToken ct)
        => Ok(await _service.CreateAsync(request, ct));
}
