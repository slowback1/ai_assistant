namespace Weather;

public class WeatherDto
{
    public string Name { get; set; } = string.Empty;
    public double TempF { get; set; }
    public double WindMph { get; set; }
    public double PrecipIn { get; set; }
    public double HeatindexF { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string ConditionIcon { get; set; } = string.Empty;
}
