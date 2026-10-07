using lab1.Domain.Entities;

namespace lab1.Application.Repositories;

public interface IXlsxAirQualityProvider
{
    Task<IReadOnlyList<AirQualityRecord>> ReadFromXlsxAsync(Stream stream);
    Task<Stream> WriteToXlsxAsync(List<AirQualityRecord> records);
}
