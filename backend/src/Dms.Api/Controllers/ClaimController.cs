using Dms.Application.Workflows;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dms.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/claims")]
public sealed class ClaimController : ControllerBase
{
    private readonly IClaimService _service;
    public ClaimController(IClaimService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid distributorId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await _service.GetAsync(distributorId, pageNumber, pageSize, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClaimRequest request, CancellationToken ct)
        => Ok(await _service.CreateAsync(request, ct));

    [HttpPost("{claimId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid claimId, CancellationToken ct)
        => Ok(await _service.ApproveAsync(claimId, ct));

    [HttpPost("{claimId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid claimId, CancellationToken ct)
        => Ok(await _service.RejectAsync(claimId, ct));
}
