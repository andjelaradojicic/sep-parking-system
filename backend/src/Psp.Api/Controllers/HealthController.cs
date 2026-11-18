using Microsoft.AspNetCore.Mvc;
using Psp.Api.Data;

namespace Psp.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController(PspDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() =>
        Ok(new { service = "Psp.Api", database = await db.Database.CanConnectAsync() });
}
