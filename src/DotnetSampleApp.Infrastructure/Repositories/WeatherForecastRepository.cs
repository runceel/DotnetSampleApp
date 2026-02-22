namespace DotnetSampleApp.Infrastructure.Repositories;

using DotnetSampleApp.Domain.Entities;
using DotnetSampleApp.Domain.Repositories;
using DotnetSampleApp.Domain.ValueObjects;
using DotnetSampleApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class WeatherForecastRepository(AppDbContext dbContext) : IWeatherForecastRepository
{
    public async Task<IReadOnlyList<WeatherForecast>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.WeatherForecasts.ToListAsync(cancellationToken);

    public async Task<WeatherForecast?> GetByIdAsync(WeatherForecastId id, CancellationToken cancellationToken = default)
        => await dbContext.WeatherForecasts
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public async Task AddAsync(WeatherForecast forecast, CancellationToken cancellationToken = default)
        => await dbContext.WeatherForecasts.AddAsync(forecast, cancellationToken);
}
