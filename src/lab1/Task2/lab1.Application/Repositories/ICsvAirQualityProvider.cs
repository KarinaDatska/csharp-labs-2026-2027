using lab1.Domain.Entities;

namespace lab1.Application.Repositories;

public interface ICsvAirQualityProvider
{
    Task<IReadOnlyList<AirQualityRecord>> ReadFromCsvAsync(Stream stream);
    Task<Stream> WriteToCsvAsync(List<AirQualityRecord> records);
}
