using System.Security.Claims;
using Dms.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dms.Api.Controllers;

[ApiController]
[Route("api/distributors")]
[Authorize]
public sealed class DistributorController : ControllerBase
{
    private readonly IDistributorService _service;

    public DistributorController(IDistributorService service) => _service = service;

    [HttpGet("my")]
    public async Task<IActionResult> GetMine(CancellationToken ct)
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException();
        return Ok(await _service.GetForUserAsync(Guid.Parse(sub), ct));
    }
}
