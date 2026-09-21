using FirstMilestone.Api.Data;
using FirstMilestone.Api.DTOs;
using FirstMilestone.Api.Entities;
using FirstMilestone.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FirstMilestone.Api.Controllers;

[ApiController]
[Route("api/weather")]
public class WeatherController(IWeatherService weatherService, AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<WeatherResponse>> Get([FromQuery] string city = "Chennai", CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(city) || city.Length > 160)
            return BadRequest(new { message = "City must contain between 1 and 160 characters." });

        var result = await weatherService.GetWeatherAsync(city.Trim(), cancellationToken);
        if (result is null) return NotFound(new { message = $"Could not find weather for '{city}'." });

        db.WeatherSearches.Add(new WeatherSearch
        {
            City = result.City,
            Latitude = result.Latitude,
            Longitude = result.Longitude
        });
        await db.SaveChangesAsync(cancellationToken);

        return Ok(result);
    }
}
