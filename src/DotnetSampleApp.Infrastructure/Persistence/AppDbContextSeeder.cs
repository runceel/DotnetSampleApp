namespace DotnetSampleApp.Infrastructure.Persistence;

using DotnetSampleApp.Domain.Entities;
using DotnetSampleApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

public static class AppDbContextSeeder
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild",
        "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    public static async Task SeedAsync(AppDbContext dbContext)
    {
        if (await dbContext.WeatherForecasts.AnyAsync())
            return;

        var forecasts = Enumerable.Range(1, 5).Select(index =>
            WeatherForecast.Create(
                WeatherForecastId.Create(),
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                new Temperature(Random.Shared.Next(-20, 55), TemperatureUnit.Celsius),
                Summaries[Random.Shared.Next(Summaries.Length)]));

        await dbContext.WeatherForecasts.AddRangeAsync(forecasts);

        var attendees = new[]
        {
            Attendee.Create("alice"),
            Attendee.Create("bob"),
            Attendee.Create("charlie"),
            Attendee.Create("diana"),
            Attendee.Create("eve"),
        };

        await dbContext.Attendees.AddRangeAsync(attendees);

        await dbContext.SaveChangesAsync();
    }
}
