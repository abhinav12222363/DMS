using Dms.Application.Reports;
using Dms.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dms.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/reports")]
public sealed class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService) => _reportService = reportService;

    [HttpGet("sales")]
    public async Task<IActionResult> Sales([FromQuery] DateOnly fromDate, [FromQuery] DateOnly toDate, [FromQuery] Guid distributorId, CancellationToken ct)
        => Ok(await _reportService.GetSalesAsync(new ReportFilter(fromDate, toDate, distributorId), ct));

    [HttpGet("orders")]
    public async Task<IActionResult> Orders([FromQuery] Guid distributorId, CancellationToken ct)
        => Ok(await _reportService.GetOrdersAsync(distributorId, ct));

    [HttpGet("stock")]
    public async Task<IActionResult> Stock([FromQuery] Guid distributorId, CancellationToken ct)
        => Ok(await _reportService.GetStockAsync(distributorId, ct));

    [HttpGet("claims")]
    public async Task<IActionResult> Claims([FromQuery] Guid distributorId, CancellationToken ct)
        => Ok(await _reportService.GetClaimsAsync(distributorId, ct));

    [HttpGet("schemes")]
    public async Task<IActionResult> Schemes(CancellationToken ct)
        => Ok(await _reportService.GetSchemesAsync(ct));
}
