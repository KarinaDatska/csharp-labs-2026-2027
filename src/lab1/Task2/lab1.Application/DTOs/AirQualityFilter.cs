namespace lab1.Application.DTOs;

public class AirQualityFilter
{
    public string? City { get; set; }
    public string ValueField { get; set; } = "AveragePollution";
    public int? MinRank { get; set; }
    public int? MaxRank { get; set; }
    public double? MinAveragePollution { get; set; }
    public double? MaxAveragePollution { get; set; }
}
