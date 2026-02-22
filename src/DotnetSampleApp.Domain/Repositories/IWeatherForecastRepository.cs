namespace DotnetSampleApp.Domain.Repositories;

using DotnetSampleApp.Domain.Entities;
using DotnetSampleApp.Domain.ValueObjects;

public interface IWeatherForecastRepository
{
    Task<IReadOnlyList<WeatherForecast>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<WeatherForecast?> GetByIdAsync(WeatherForecastId id, CancellationToken cancellationToken = default);
    Task AddAsync(WeatherForecast forecast, CancellationToken cancellationToken = default);
}
