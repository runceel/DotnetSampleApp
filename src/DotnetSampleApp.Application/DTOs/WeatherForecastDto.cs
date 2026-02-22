namespace DotnetSampleApp.Application.DTOs;

public record WeatherForecastDto(
    Guid Id,
    DateOnly Date,
    int TemperatureC,
    int TemperatureF,
    string? Summary);
