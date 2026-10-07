using lab1.Application.DTOs;

namespace lab1.Application.Services;

public interface IJsonAirQualityService
{
    Task<IReadOnlyList<AirQualityDto>> ReadFromJsonAsync(Stream stream);
    Task<Stream> WriteToJsonAsync();
}
