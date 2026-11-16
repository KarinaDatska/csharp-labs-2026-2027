using lab1.Application.DTOs;

namespace lab1.Application.Services;

public interface IXlsxAirQualityService
{
    Task<IReadOnlyList<AirQualityDto>> ReadFromXlsxAsync(Stream stream);
    Task<Stream> WriteToXlsxAsync();
}
