using lab1.Application.DTOs;
using lab1.Application.Repositories;
using lab1.Domain.Entities;

namespace lab1.Application.Services;

public class ChartDataService : IChartDataService
{
    private readonly IDataAnalyticsService _dataAnalyticsService;
    private readonly IScottPlotChartService _chartService;

    public ChartDataService(
        IDataAnalyticsService dataAnalyticsService,
        IScottPlotChartService chartService)
    {
        _dataAnalyticsService = dataAnalyticsService;
        _chartService = chartService;
    }

    public byte[]? GetTopPollutedBarChart(AirQualityFilter filter)
    {
        var records = SelectValues(GetFilteredRecords(filter), filter.ValueField);
        if (records.Count == 0) return null;

        return _chartService.CreateTopPollutedBarChart(records);
    }

    public byte[]? GetPollutionDistributionPieChart(AirQualityFilter filter)
    {
        var records = SelectValues(GetFilteredRecords(filter), filter.ValueField);
        if (records.Count == 0) return null;

        return _chartService.CreatePollutionDistributionPieChart(records);
    }

    public byte[]? GetMonthlyTrendLineChart(AirQualityFilter filter)
    {
        var records = GetFilteredRecords(filter);
        if (records.Count == 0) return null;

        return _chartService.CreateMonthlyTrendLineChart(records);
    }

    private List<AirQualityRecord> GetFilteredRecords(AirQualityFilter filter)
    {
        return _dataAnalyticsService.FilterRecords(filter).ToList();
    }

    private static List<AirQualityRecord> SelectValues(List<AirQualityRecord> records, string field)
    {
        if (field == "AveragePollution") return records;
        var property = typeof(MonthlyMeasurements).GetProperties()
            .FirstOrDefault(p => p.Name.Equals(field, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException("Невідоме поле графіка.");
        return records.Where(r => property.GetValue(r.Measurements) != null)
            .Select(r => new AirQualityRecord(r.Id, r.Rank, r.CityAndCountry,
                (double)property.GetValue(r.Measurements)!, r.Measurements)).ToList();
    }
}
