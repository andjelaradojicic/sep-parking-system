using Microsoft.AspNetCore.Mvc;
using Parking.Api.Data;

namespace Parking.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController(ParkingDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() =>
        Ok(new { service = "Parking.Api", database = await db.Database.CanConnectAsync() });
}
