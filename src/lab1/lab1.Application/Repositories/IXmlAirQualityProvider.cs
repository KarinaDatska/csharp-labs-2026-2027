using lab1.Domain.Entities;

namespace lab1.Application.Repositories;

public interface IXmlAirQualityProvider
{
    Task<IReadOnlyList<AirQualityRecord>> ReadFromXmlAsync(Stream stream);
    Task<Stream> WriteToXmlAsync(List<AirQualityRecord> records);
}
