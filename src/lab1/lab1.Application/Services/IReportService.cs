using lab1.Application.DTOs;

namespace lab1.Application.Services;

public interface IReportService
{
    Task<Stream> CreateXlsxReportAsync();
    Task<Stream> CreateDocxReportAsync(ReportMetadataDto metadata);
}
