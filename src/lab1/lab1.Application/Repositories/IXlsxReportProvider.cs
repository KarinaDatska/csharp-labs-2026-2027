using lab1.Application.DTOs;
using lab1.Domain.Entities;

namespace lab1.Application.Repositories;

public interface IXlsxReportProvider
{
    Task<Stream> CreateReportAsync(
        IReadOnlyList<AirQualityRecord> records,
        ReportDataDto reportData);
}
