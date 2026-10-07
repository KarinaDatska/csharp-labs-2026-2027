using System.Text.Json;
using lab1.Application.Repositories;
using lab1.Domain.Entities;

namespace lab1.Infrastructure.Repositories;

public class JsonAirQualityProvider : IJsonAirQualityProvider
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public async Task<IReadOnlyList<AirQualityRecord>> ReadFromJsonAsync(Stream stream)
    {
        var records = await JsonSerializer.DeserializeAsync<List<AirQualityRecord>>(stream, JsonOptions);
        return records ?? [];
    }

    public async Task<Stream> WriteToJsonAsync(List<AirQualityRecord> records)
    {
        var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, records, JsonOptions);
        stream.Position = 0;
        return stream;
    }
}