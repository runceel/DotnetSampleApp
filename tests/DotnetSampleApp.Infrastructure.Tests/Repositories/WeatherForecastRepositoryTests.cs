namespace DotnetSampleApp.Infrastructure.Tests.Repositories;

using DotnetSampleApp.Domain.Entities;
using DotnetSampleApp.Domain.ValueObjects;
using DotnetSampleApp.Infrastructure.Persistence;
using DotnetSampleApp.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

[TestClass]
public class WeatherForecastRepositoryTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [TestMethod]
    public async Task AddAsync_GetAllAsync_追加した予報が取得できること()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var repository = new WeatherForecastRepository(dbContext);
        var forecast = WeatherForecast.Create(
            WeatherForecastId.Create(),
            DateOnly.FromDateTime(DateTime.Now),
            new Temperature(25, TemperatureUnit.Celsius),
            "Warm");

        // Act
        await repository.AddAsync(forecast);
        await dbContext.SaveChangesAsync();
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result[0].Summary.Should().Be("Warm");
    }

    [TestMethod]
    public async Task GetByIdAsync_存在するIDの場合_予報を返すこと()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var repository = new WeatherForecastRepository(dbContext);
        var id = WeatherForecastId.Create();
        var forecast = WeatherForecast.Create(
            id,
            DateOnly.FromDateTime(DateTime.Now),
            new Temperature(10, TemperatureUnit.Celsius),
            "Cool");

        await repository.AddAsync(forecast);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [TestMethod]
    public async Task GetByIdAsync_存在しないIDの場合_nullを返すこと()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var repository = new WeatherForecastRepository(dbContext);

        // Act
        var result = await repository.GetByIdAsync(WeatherForecastId.Create());

        // Assert
        result.Should().BeNull();
    }
}
