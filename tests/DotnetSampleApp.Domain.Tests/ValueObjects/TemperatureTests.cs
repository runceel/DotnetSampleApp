namespace DotnetSampleApp.Domain.Tests.ValueObjects;

using DotnetSampleApp.Domain.ValueObjects;
using FluentAssertions;

[TestClass]
public class TemperatureTests
{
    [TestMethod]
    public void ToFahrenheit_摂氏0度の場合_32を返すこと()
    {
        // Arrange
        var temperature = new Temperature(0, TemperatureUnit.Celsius);

        // Act
        var result = temperature.ToFahrenheit();

        // Assert
        result.Should().Be(32);
    }

    [TestMethod]
    public void ToCelsius_華氏32度の場合_0を返すこと()
    {
        // Arrange
        var temperature = new Temperature(32, TemperatureUnit.Fahrenheit);

        // Act
        var result = temperature.ToCelsius();

        // Assert
        result.Should().Be(0);
    }

    [TestMethod]
    public void ToCelsius_摂氏の場合_そのまま返すこと()
    {
        // Arrange
        var temperature = new Temperature(25, TemperatureUnit.Celsius);

        // Act
        var result = temperature.ToCelsius();

        // Assert
        result.Should().Be(25);
    }

    [TestMethod]
    public void ToFahrenheit_華氏の場合_そのまま返すこと()
    {
        // Arrange
        var temperature = new Temperature(77, TemperatureUnit.Fahrenheit);

        // Act
        var result = temperature.ToFahrenheit();

        // Assert
        result.Should().Be(77);
    }
}
