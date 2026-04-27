using Dms.Application.Workflows;
using Microsoft.AspNetCore.Mvc;

namespace Dms.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class ErpIntegrationController : ControllerBase
{
    private readonly IErpIntegrationService _service;
    public ErpIntegrationController(IErpIntegrationService service) => _service = service;

    [HttpPost("token")]
    public async Task<IActionResult> Token([FromBody] ErpTokenRequest request, CancellationToken ct)
        => Ok(new { token = await _service.IssueTokenAsync(request, ct) });

    [HttpPost("item")]
    public async Task<IActionResult> Item([FromBody] ErpItemUpsertRequest request, [FromHeader(Name = "X-ERP-Token")] string token, CancellationToken ct)
    {
        await _service.UpsertItemAsync(request, token, ct);
        return Accepted();
    }

    [HttpPost("distributor")]
    public async Task<IActionResult> Distributor([FromBody] ErpDistributorUpsertRequest request, [FromHeader(Name = "X-ERP-Token")] string token, CancellationToken ct)
    {
        await _service.UpsertDistributorAsync(request, token, ct);
        return Accepted();
    }

    [HttpPost("hsn")]
    public async Task<IActionResult> Hsn([FromBody] ErpHsnUpsertRequest request, [FromHeader(Name = "X-ERP-Token")] string token, CancellationToken ct)
    {
        await _service.UpsertHsnAsync(request, token, ct);
        return Accepted();
    }

    [HttpPost("sales-invoice")]
    public async Task<IActionResult> Invoice([FromBody] ErpInvoicePushRequest request, [FromHeader(Name = "X-ERP-Token")] string token, CancellationToken ct)
    {
        await _service.PushSalesInvoiceAsync(request, token, ct);
        return Accepted();
    }
}
