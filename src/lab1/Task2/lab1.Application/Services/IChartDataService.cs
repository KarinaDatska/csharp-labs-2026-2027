using lab1.Application.DTOs;

namespace lab1.Application.Services;

public interface IChartDataService
{
    byte[]? GetTopPollutedBarChart(AirQualityFilter filter);
    byte[]? GetPollutionDistributionPieChart(AirQualityFilter filter);
    byte[]? GetMonthlyTrendLineChart(AirQualityFilter filter);
}
