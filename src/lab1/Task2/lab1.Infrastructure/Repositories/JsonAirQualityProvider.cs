using System.Text.Json;
using lab1.Application.Repositories;
using lab1.Domain.Entities;

namespace lab1.Infrastructure.Repositories;

public class JsonAirQualityProvider(lab1.Application.DTOs.FileOptions options) : IJsonAirQualityProvider
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public async Task<IReadOnlyList<AirQualityRecord>> ReadFromJsonAsync(Stream stream)
    {
        using var reader = new StreamReader(stream, options.GetEncoding(), detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        var records = JsonSerializer.Deserialize<List<AirQualityRecord>>(await reader.ReadToEndAsync(), JsonOptions);
        return records ?? [];
    }

    public async Task<Stream> WriteToJsonAsync(List<AirQualityRecord> records)
    {
        var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream, options.GetEncoding(), leaveOpen: true);
        await writer.WriteAsync(JsonSerializer.Serialize(records, JsonOptions));
        await writer.FlushAsync();
        stream.Position = 0;
        return stream;
    }
}
