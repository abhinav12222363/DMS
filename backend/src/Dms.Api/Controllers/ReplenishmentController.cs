using Dms.Application.Workflows;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dms.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/replenishment")]
public sealed class ReplenishmentController : ControllerBase
{
    private readonly IReplenishmentService _service;
    public ReplenishmentController(IReplenishmentService service) => _service = service;

    [HttpPost("run")]
    public async Task<IActionResult> Run(CancellationToken ct)
    {
        await _service.RunAutoSuggestAsync(ct);
        return Accepted();
    }

    [HttpGet("suggestions")]
    public async Task<IActionResult> Suggestions([FromQuery] Guid distributorId, CancellationToken ct)
        => Ok(await _service.GetSuggestionsAsync(distributorId, ct));
}
