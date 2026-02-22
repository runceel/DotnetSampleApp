namespace DotnetSampleApp.Domain.Entities;

using DotnetSampleApp.Domain.ValueObjects;

public class WeatherForecast
{
    public WeatherForecastId Id { get; private set; }
    public DateOnly Date { get; private set; }
    public Temperature Temperature { get; private set; }
    public string? Summary { get; private set; }

    private WeatherForecast()
    {
        Id = null!;
        Temperature = null!;
    }

    public static WeatherForecast Create(
        WeatherForecastId id,
        DateOnly date,
        Temperature temperature,
        string? summary)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(temperature);

        return new WeatherForecast
        {
            Id = id,
            Date = date,
            Temperature = temperature,
            Summary = summary
        };
    }

    public void UpdateSummary(string? summary)
    {
        Summary = summary;
    }

    public void UpdateTemperature(Temperature temperature)
    {
        ArgumentNullException.ThrowIfNull(temperature);
        Temperature = temperature;
    }
}
