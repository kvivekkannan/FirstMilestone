using FirstMilestone.Api.Data;
using FirstMilestone.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstMilestone.Api.Controllers;

[ApiController]
[Route("api/modules")]
public class ModulesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModuleResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var modules = await db.DashboardModules.AsNoTracking().OrderBy(x => x.Id).Select(x =>
            new ModuleResponse(x.Id, x.Key, x.Name, x.Description, x.Icon, x.IsInstalled)).ToListAsync(cancellationToken);
        return Ok(modules);
    }

    [HttpPost("{key}/install")]
    public async Task<IActionResult> Install(string key, CancellationToken cancellationToken)
        => await SetInstalled(key, true, cancellationToken);

    [HttpPost("{key}/uninstall")]
    public async Task<IActionResult> Uninstall(string key, CancellationToken cancellationToken)
        => await SetInstalled(key, false, cancellationToken);

    private async Task<IActionResult> SetInstalled(string key, bool installed, CancellationToken cancellationToken)
    {
        var module = await db.DashboardModules.FirstOrDefaultAsync(x => x.Key == key, cancellationToken);
        if (module is null) return NotFound(new { message = "Module not found." });
        module.IsInstalled = installed;
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
