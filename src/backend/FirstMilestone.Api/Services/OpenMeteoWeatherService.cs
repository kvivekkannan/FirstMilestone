using System.Text.Json;
using System.Text.Json.Serialization;
using FirstMilestone.Api.DTOs;

namespace FirstMilestone.Api.Services;

public sealed class OpenMeteoWeatherService(HttpClient httpClient, ILogger<OpenMeteoWeatherService> logger) : IWeatherService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<WeatherResponse?> GetWeatherAsync(string city, CancellationToken cancellationToken)
    {
        var locationResponse = await httpClient.GetAsync(
            $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(city)}&count=1&language=en&format=json",
            cancellationToken);

        if (!locationResponse.IsSuccessStatusCode)
        {
            logger.LogWarning("Open-Meteo geocoding failed with status {StatusCode}", locationResponse.StatusCode);
            return null;
        }

        var locationJson = await locationResponse.Content.ReadAsStringAsync(cancellationToken);
        var locations = JsonSerializer.Deserialize<GeocodingResponse>(locationJson, JsonOptions);
        var location = locations?.Results?.FirstOrDefault();
        if (location is null) return null;

        var forecastUrl = "https://api.open-meteo.com/v1/forecast" +
                          $"?latitude={location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                          $"&longitude={location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                          "&current=temperature_2m,relative_humidity_2m,apparent_temperature,weather_code,wind_speed_10m" +
                          "&daily=weather_code,temperature_2m_max,temperature_2m_min" +
                          "&forecast_days=5&timezone=auto";

        var forecastResponse = await httpClient.GetAsync(forecastUrl, cancellationToken);
        forecastResponse.EnsureSuccessStatusCode();
        var forecastJson = await forecastResponse.Content.ReadAsStringAsync(cancellationToken);
        var forecast = JsonSerializer.Deserialize<ForecastResponse>(forecastJson, JsonOptions)
                       ?? throw new InvalidOperationException("Open-Meteo returned an empty forecast.");

        var daily = new List<DailyForecast>();
        for (var i = 0; i < forecast.Daily.Time.Count; i++)
        {
            daily.Add(new DailyForecast(
                DateTime.Parse(forecast.Daily.Time[i]),
                forecast.Daily.Temperature2mMin[i],
                forecast.Daily.Temperature2mMax[i],
                forecast.Daily.WeatherCode[i],
                WeatherCodeMapper.Describe(forecast.Daily.WeatherCode[i])));
        }

        return new WeatherResponse(
            location.Name,
            location.Country,
            location.Latitude,
            location.Longitude,
            forecast.Current.Temperature2m,
            forecast.Current.ApparentTemperature,
            forecast.Current.RelativeHumidity2m,
            forecast.Current.WindSpeed10m,
            forecast.Current.WeatherCode,
            WeatherCodeMapper.Describe(forecast.Current.WeatherCode),
            DateTime.UtcNow,
            daily);
    }

    private sealed class GeocodingResponse { [JsonPropertyName("results")] public List<GeocodingResult>? Results { get; set; } }
    private sealed class GeocodingResult { [JsonPropertyName("name")] public string Name { get; set; } = ""; [JsonPropertyName("country")] public string Country { get; set; } = ""; [JsonPropertyName("latitude")] public double Latitude { get; set; } [JsonPropertyName("longitude")] public double Longitude { get; set; } }
    private sealed class ForecastResponse { [JsonPropertyName("current")] public CurrentData Current { get; set; } = new(); [JsonPropertyName("daily")] public DailyData Daily { get; set; } = new(); }
    private sealed class CurrentData { [JsonPropertyName("temperature_2m")] public double Temperature2m { get; set; } [JsonPropertyName("relative_humidity_2m")] public double RelativeHumidity2m { get; set; } [JsonPropertyName("apparent_temperature")] public double ApparentTemperature { get; set; } [JsonPropertyName("weather_code")] public int WeatherCode { get; set; } [JsonPropertyName("wind_speed_10m")] public double WindSpeed10m { get; set; } }
    private sealed class DailyData { [JsonPropertyName("time")] public List<string> Time { get; set; } = []; [JsonPropertyName("temperature_2m_max")] public List<double> Temperature2mMax { get; set; } = []; [JsonPropertyName("temperature_2m_min")] public List<double> Temperature2mMin { get; set; } = []; [JsonPropertyName("weather_code")] public List<int> WeatherCode { get; set; } = []; }
}

public static class WeatherCodeMapper
{
    public static string Describe(int code) => code switch
    {
        0 => "Clear sky",
        1 or 2 or 3 => "Partly cloudy",
        45 or 48 => "Fog",
        51 or 53 or 55 or 56 or 57 => "Drizzle",
        61 or 63 or 65 or 66 or 67 => "Rain",
        71 or 73 or 75 or 77 => "Snow",
        80 or 81 or 82 => "Rain showers",
        85 or 86 => "Snow showers",
        95 => "Thunderstorm",
        96 or 99 => "Thunderstorm with hail",
        _ => "Unknown conditions"
    };
}
