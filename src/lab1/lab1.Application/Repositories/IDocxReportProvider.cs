using lab1.Application.DTOs;

namespace lab1.Application.Repositories;

public interface IDocxReportProvider
{
    Task<Stream> CreateReportAsync(
        ReportDataDto reportData,
        ReportMetadataDto metadata,
        byte[]? topPollutedChart,
        byte[]? distributionChart,
        byte[]? monthlyTrendChart);
}
