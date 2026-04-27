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
}
