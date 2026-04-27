using Dms.Application.Workflows;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dms.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/approval/orders")]
public sealed class ApprovalController : ControllerBase
{
    private readonly IApprovalService _service;
    public ApprovalController(IApprovalService service) => _service = service;

    [HttpPost("{orderId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid orderId, [FromBody] OrderApprovalActionRequest request, CancellationToken ct)
        => Ok(await _service.ApproveOrderAsync(orderId, request, ct));

    [HttpPost("{orderId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid orderId, [FromBody] OrderApprovalActionRequest request, CancellationToken ct)
        => Ok(await _service.RejectOrderAsync(orderId, request, ct));
}
