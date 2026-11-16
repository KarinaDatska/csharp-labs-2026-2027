using lab1.Application.DTOs;

namespace lab1.Application.Services;

public interface IXmlAirQualityService
{
    Task<IReadOnlyList<AirQualityDto>> ReadFromXmlAsync(Stream stream);
    Task<Stream> WriteToXmlAsync();
}
