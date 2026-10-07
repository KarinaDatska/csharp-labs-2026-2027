using lab1.Application.DTOs;
using lab1.Application.Repositories;
using lab1.Domain.Entities;

namespace lab1.Application.Services;

public class ReportService : IReportService
{
    private readonly IDataAnalyticsService _analyticsService;
    private readonly IChartDataService _chartDataService;
    private readonly IXlsxReportProvider _xlsxReportProvider;
    private readonly IDocxReportProvider _docxReportProvider;

    public ReportService(
        IDataAnalyticsService analyticsService,
        IChartDataService chartDataService,
        IXlsxReportProvider xlsxReportProvider,
        IDocxReportProvider docxReportProvider)
    {
        _analyticsService = analyticsService;
        _chartDataService = chartDataService;
        _xlsxReportProvider = xlsxReportProvider;
        _docxReportProvider = docxReportProvider;
    }

    public Task<Stream> CreateXlsxReportAsync()
    {
        var records = _analyticsService.GetAllRecords().ToList();
        var reportData = BuildReportData(records);

        return _xlsxReportProvider.CreateReportAsync(records, reportData);
    }

    public Task<Stream> CreateDocxReportAsync(ReportMetadataDto metadata)
    {
        var records = _analyticsService.GetAllRecords().ToList();
        var reportData = BuildReportData(records);

        var filter = new AirQualityFilter();
        var topPollutedChart = _chartDataService.GetTopPollutedBarChart(filter);
        var distributionChart = _chartDataService.GetPollutionDistributionPieChart(filter);
        var monthlyTrendChart = _chartDataService.GetMonthlyTrendLineChart(filter);

        return _docxReportProvider.CreateReportAsync(
            reportData,
            metadata,
            topPollutedChart,
            distributionChart,
            monthlyTrendChart);
    }

    public static ReportDataDto BuildReportData(IReadOnlyList<AirQualityRecord> records)
    {
        if (records.Count == 0)
        {
            return new ReportDataDto();
        }

        var total = records.Count;
        var avgMean = records.Average(r => r.AveragePollution);
        var sorted = records.Select(r => r.AveragePollution).OrderBy(v => v).ToList();
        var median = sorted.Count % 2 == 0
            ? (sorted[sorted.Count / 2 - 1] + sorted[sorted.Count / 2]) / 2.0
            : sorted[sorted.Count / 2];

        var min = records.Min(r => r.AveragePollution);
        var max = records.Max(r => r.AveragePollution);

        var categories = new List<ReportGroupRowDto>
        {
            CreateCategoryRow("Добра (<= 50)", records.Where(r => r.AveragePollution <= 50).ToList(), total),
            CreateCategoryRow("Помірна (51-100)", records.Where(r => r.AveragePollution > 50 && r.AveragePollution <= 100).ToList(), total),
            CreateCategoryRow("Шкідлива (101-150)", records.Where(r => r.AveragePollution > 100 && r.AveragePollution <= 150).ToList(), total),
            CreateCategoryRow("Дуже шкідлива (> 150)", records.Where(r => r.AveragePollution > 150).ToList(), total)
        };

        var monthlyAverages = new List<MonthlyAverageDto>
        {
            new("Січень", AverageOrZero(records.Select(r => r.Measurements?.January))),
            new("Лютий", AverageOrZero(records.Select(r => r.Measurements?.February))),
            new("Березень", AverageOrZero(records.Select(r => r.Measurements?.March))),
            new("Квітень", AverageOrZero(records.Select(r => r.Measurements?.April))),
            new("Травень", AverageOrZero(records.Select(r => r.Measurements?.May))),
            new("Червень", AverageOrZero(records.Select(r => r.Measurements?.June))),
            new("Липень", AverageOrZero(records.Select(r => r.Measurements?.July))),
            new("Серпень", AverageOrZero(records.Select(r => r.Measurements?.August))),
            new("Вересень", AverageOrZero(records.Select(r => r.Measurements?.September))),
            new("Жовтень", AverageOrZero(records.Select(r => r.Measurements?.October))),
            new("Листопад", AverageOrZero(records.Select(r => r.Measurements?.November))),
            new("Грудень", AverageOrZero(records.Select(r => r.Measurements?.December)))
        };

        return new ReportDataDto
        {
            TotalRecords = total,
            AveragePollutionMean = Math.Round(avgMean, 2),
            MedianPollution = Math.Round(median, 2),
            MinPollution = Math.Round(min, 2),
            MaxPollution = Math.Round(max, 2),
            ByCategory = categories,
            MonthlyAverages = monthlyAverages
        };
    }

    private static ReportGroupRowDto CreateCategoryRow(string name, List<AirQualityRecord> subset, int total)
    {
        var count = subset.Count;
        var percentage = total > 0 ? Math.Round((double)count / total * 100.0, 2) : 0;
        var avg = count > 0 ? Math.Round(subset.Average(r => r.AveragePollution), 2) : 0;
        return new ReportGroupRowDto(name, count, percentage, avg);
    }

    private static double AverageOrZero(IEnumerable<double?> values)
    {
        var valid = values.Where(v => v.HasValue).Select(v => v!.Value).ToList();
        return valid.Count > 0 ? Math.Round(valid.Average(), 2) : 0;
    }
}
