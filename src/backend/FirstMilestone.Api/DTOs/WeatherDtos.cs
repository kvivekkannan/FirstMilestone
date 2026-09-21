namespace FirstMilestone.Api.DTOs;

public record WeatherResponse(
    string City,
    string Country,
    double Latitude,
    double Longitude,
    double TemperatureC,
    double ApparentTemperatureC,
    double RelativeHumidity,
    double WindSpeedKmh,
    int WeatherCode,
    string Condition,
    DateTime RetrievedUtc,
    IReadOnlyList<DailyForecast> Forecast);

public record DailyForecast(DateTime Date, double MinC, double MaxC, int WeatherCode, string Condition);

public record LocationResult(string Name, string Country, double Latitude, double Longitude);
