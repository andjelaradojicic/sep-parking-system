using Microsoft.AspNetCore.Mvc;
using Bank.Api.Data;

namespace Bank.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController(BankDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() =>
        Ok(new { service = "Bank.Api", database = await db.Database.CanConnectAsync() });
}
