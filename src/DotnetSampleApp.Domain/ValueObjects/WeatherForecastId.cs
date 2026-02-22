namespace DotnetSampleApp.Domain.ValueObjects;

public record WeatherForecastId(Guid Value)
{
    public static WeatherForecastId Create() => new(Guid.NewGuid());
}
