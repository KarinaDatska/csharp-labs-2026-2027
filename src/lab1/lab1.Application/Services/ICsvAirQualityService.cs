using lab1.Application.DTOs;

namespace lab1.Application.Services;

public interface ICsvAirQualityService
{
    Task<IReadOnlyList<AirQualityDto>> ReadFromCsvAsync(Stream stream, string sourceFilePath);
    Task<Stream> WriteToCsvAsync();
}
