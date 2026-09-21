using FirstMilestone.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstMilestone.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController(AppDbContext db) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> Summary(CancellationToken cancellationToken)
    {
        var installed = await db.DashboardModules.CountAsync(x => x.IsInstalled, cancellationToken);
        var searches = await db.WeatherSearches.CountAsync(cancellationToken);
        return Ok(new { installedModules = installed, weatherSearches = searches });
    }
}
