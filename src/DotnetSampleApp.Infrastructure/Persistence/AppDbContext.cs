namespace DotnetSampleApp.Infrastructure.Persistence;

using DotnetSampleApp.Domain.Entities;
using DotnetSampleApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeatherForecast>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasConversion(
                    id => id.Value,
                    value => new WeatherForecastId(value));

            entity.OwnsOne(e => e.Temperature);
        });
    }
}
