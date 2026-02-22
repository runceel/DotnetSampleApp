namespace DotnetSampleApp.Infrastructure;

using DotnetSampleApp.Application.Interfaces;
using DotnetSampleApp.Domain.Repositories;
using DotnetSampleApp.Infrastructure.Persistence;
using DotnetSampleApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("DotnetSampleApp"));

        services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
