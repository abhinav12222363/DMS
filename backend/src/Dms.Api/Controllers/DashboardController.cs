using Dms.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dms.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service) => _service = service;

    [HttpGet("sales")]
    public async Task<IActionResult> Sales([FromQuery] Guid distributorId, [FromQuery] DateOnly fromDate, [FromQuery] DateOnly toDate, CancellationToken ct)
        => Ok(await _service.GetSalesDashboardAsync(distributorId, fromDate, toDate, ct));
}
