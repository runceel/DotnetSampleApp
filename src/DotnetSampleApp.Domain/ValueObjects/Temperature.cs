namespace DotnetSampleApp.Domain.ValueObjects;

public record Temperature(int Value, TemperatureUnit Unit)
{
    public int ToFahrenheit() => Unit switch
    {
        TemperatureUnit.Celsius => 32 + (int)(Value / 0.5556),
        TemperatureUnit.Fahrenheit => Value,
        _ => throw new ArgumentOutOfRangeException(nameof(Unit))
    };

    public int ToCelsius() => Unit switch
    {
        TemperatureUnit.Celsius => Value,
        TemperatureUnit.Fahrenheit => (int)((Value - 32) * 0.5556),
        _ => throw new ArgumentOutOfRangeException(nameof(Unit))
    };
}

public enum TemperatureUnit
{
    Celsius,
    Fahrenheit
}
