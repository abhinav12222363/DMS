using Dms.Application.Workflows;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dms.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/schemes")]
public sealed class SchemeController : ControllerBase
{
    private readonly ISchemeWorkflowService _service;
    public SchemeController(ISchemeWorkflowService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) => Ok(await _service.GetAsync(ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSchemeRequest request, CancellationToken ct) => Ok(await _service.CreateAsync(request, ct));

    [HttpPost("{schemeId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid schemeId, CancellationToken ct) => Ok(await _service.ApproveAsync(schemeId, ct));
}
