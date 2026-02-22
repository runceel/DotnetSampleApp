namespace DotnetSampleApp.Domain.Tests.Entities;

using DotnetSampleApp.Domain.Entities;
using DotnetSampleApp.Domain.ValueObjects;
using FluentAssertions;

[TestClass]
public class WeatherForecastTests
{
    [TestMethod]
    public void Create_有効なパラメータの場合_エンティティが生成されること()
    {
        // Arrange
        var id = WeatherForecastId.Create();
        var date = DateOnly.FromDateTime(DateTime.Now);
        var temperature = new Temperature(25, TemperatureUnit.Celsius);
        var summary = "Warm";

        // Act
        var forecast = WeatherForecast.Create(id, date, temperature, summary);

        // Assert
        forecast.Id.Should().Be(id);
        forecast.Date.Should().Be(date);
        forecast.Temperature.Should().Be(temperature);
        forecast.Summary.Should().Be(summary);
    }

    [TestMethod]
    public void Create_idがnullの場合_ArgumentNullExceptionをスローすること()
    {
        // Arrange & Act
        var act = () => WeatherForecast.Create(
            null!,
            DateOnly.FromDateTime(DateTime.Now),
            new Temperature(25, TemperatureUnit.Celsius),
            "Warm");

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void UpdateSummary_新しいサマリーの場合_更新されること()
    {
        // Arrange
        var forecast = WeatherForecast.Create(
            WeatherForecastId.Create(),
            DateOnly.FromDateTime(DateTime.Now),
            new Temperature(25, TemperatureUnit.Celsius),
            "Warm");

        // Act
        forecast.UpdateSummary("Hot");

        // Assert
        forecast.Summary.Should().Be("Hot");
    }

    [TestMethod]
    public void UpdateTemperature_新しい温度の場合_更新されること()
    {
        // Arrange
        var forecast = WeatherForecast.Create(
            WeatherForecastId.Create(),
            DateOnly.FromDateTime(DateTime.Now),
            new Temperature(25, TemperatureUnit.Celsius),
            "Warm");
        var newTemperature = new Temperature(35, TemperatureUnit.Celsius);

        // Act
        forecast.UpdateTemperature(newTemperature);

        // Assert
        forecast.Temperature.Should().Be(newTemperature);
    }
}
