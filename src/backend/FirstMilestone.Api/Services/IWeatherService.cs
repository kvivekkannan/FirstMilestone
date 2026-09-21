using FirstMilestone.Api.DTOs;

namespace FirstMilestone.Api.Services;

public interface IWeatherService
{
    Task<WeatherResponse?> GetWeatherAsync(string city, CancellationToken cancellationToken);
}
