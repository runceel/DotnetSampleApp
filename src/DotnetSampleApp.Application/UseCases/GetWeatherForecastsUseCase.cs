namespace DotnetSampleApp.Application.UseCases;

using DotnetSampleApp.Application.DTOs;
using DotnetSampleApp.Domain.Repositories;

public class GetWeatherForecastsUseCase(IWeatherForecastRepository repository)
{
    public async Task<IReadOnlyList<WeatherForecastDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var forecasts = await repository.GetAllAsync(cancellationToken);

        return forecasts
            .Select(f => new WeatherForecastDto(
                f.Id.Value,
                f.Date,
                f.Temperature.ToCelsius(),
                f.Temperature.ToFahrenheit(),
                f.Summary))
            .ToList();
    }
}
