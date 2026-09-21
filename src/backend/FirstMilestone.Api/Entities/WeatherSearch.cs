namespace FirstMilestone.Api.Entities;

public class WeatherSearch
{
    public long Id { get; set; }
    public string City { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime SearchedUtc { get; set; } = DateTime.UtcNow;
}
