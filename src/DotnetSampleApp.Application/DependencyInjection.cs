namespace DotnetSampleApp.Application;

using DotnetSampleApp.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetWeatherForecastsUseCase>();
        return services;
    }
}
