using lab1.Domain.Entities;

namespace lab1.Application.Repositories;

public interface IJsonAirQualityProvider
{
    Task<IReadOnlyList<AirQualityRecord>> ReadFromJsonAsync(Stream stream);
    Task<Stream> WriteToJsonAsync(List<AirQualityRecord> records);
}
