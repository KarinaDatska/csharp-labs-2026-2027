namespace lab1.Application.DTOs;

public class ReportDataDto
{
    public int TotalRecords { get; set; }
    public double AveragePollutionMean { get; set; }
    public double MedianPollution { get; set; }
    public double MinPollution { get; set; }
    public double MaxPollution { get; set; }
    public IReadOnlyList<ReportGroupRowDto> ByCategory { get; set; } = [];
    public IReadOnlyList<MonthlyAverageDto> MonthlyAverages { get; set; } = [];
}
