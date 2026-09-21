using FirstMilestone.Api.Services;

namespace FirstMilestone.Api.Tests;

public class WeatherCodeMapperTests
{
    [Theory]
    [InlineData(0, "Clear sky")]
    [InlineData(61, "Rain")]
    [InlineData(95, "Thunderstorm")]
    public void Describe_ReturnsExpectedCondition(int code, string expected)
    {
        Assert.Equal(expected, WeatherCodeMapper.Describe(code));
    }
}
