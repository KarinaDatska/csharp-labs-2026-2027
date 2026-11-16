namespace lab1.Application.DTOs;

public class AirQualityDto
{
    public int Id { get; set; }
    public int Rank { get; set; }
    public string CityAndCountry { get; set; } = string.Empty;
    public double AveragePollution { get; set; }
    public MonthlyMeasurementsDto Measurements { get; set; } = new();
}

public class MonthlyMeasurementsDto
{
    public double? January { get; set; }
    public double? February { get; set; }
    public double? March { get; set; }
    public double? April { get; set; }
    public double? May { get; set; }
    public double? June { get; set; }
    public double? July { get; set; }
    public double? August { get; set; }
    public double? September { get; set; }
    public double? October { get; set; }
    public double? November { get; set; }
    public double? December { get; set; }
}