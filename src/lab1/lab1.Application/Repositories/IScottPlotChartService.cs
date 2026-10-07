using lab1.Domain.Entities;

namespace lab1.Application.Repositories;

public interface IScottPlotChartService
{
    byte[] CreateTopPollutedBarChart(IEnumerable<AirQualityRecord> records);
    byte[] CreatePollutionDistributionPieChart(IEnumerable<AirQualityRecord> records);
    byte[] CreateMonthlyTrendLineChart(IEnumerable<AirQualityRecord> records);
}
