namespace DotnetSampleApp.Application.Tests.UseCases;

using DotnetSampleApp.Application.UseCases;
using DotnetSampleApp.Domain.Entities;
using DotnetSampleApp.Domain.Repositories;
using DotnetSampleApp.Domain.ValueObjects;
using FluentAssertions;
using Moq;

[TestClass]
public class GetWeatherForecastsUseCaseTests
{
    private readonly Mock<IWeatherForecastRepository> _repositoryMock = new();
    private GetWeatherForecastsUseCase _sut = null!;

    [TestInitialize]
    public void Setup()
    {
        _sut = new GetWeatherForecastsUseCase(_repositoryMock.Object);
    }

    [TestMethod]
    public async Task ExecuteAsync_データが存在する場合_DTOリストを返すこと()
    {
        // Arrange
        var forecasts = new List<WeatherForecast>
        {
            WeatherForecast.Create(
                WeatherForecastId.Create(),
                DateOnly.FromDateTime(DateTime.Now),
                new Temperature(25, TemperatureUnit.Celsius),
                "Warm"),
            WeatherForecast.Create(
                WeatherForecastId.Create(),
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                new Temperature(10, TemperatureUnit.Celsius),
                "Cool")
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecasts);

        // Act
        var result = await _sut.ExecuteAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Summary.Should().Be("Warm");
        result[0].TemperatureC.Should().Be(25);
        result[1].Summary.Should().Be("Cool");
    }

    [TestMethod]
    public async Task ExecuteAsync_データが空の場合_空リストを返すこと()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _sut.ExecuteAsync();

        // Assert
        result.Should().BeEmpty();
    }
}
