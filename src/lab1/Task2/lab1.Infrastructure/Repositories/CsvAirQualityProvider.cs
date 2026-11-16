using CsvHelper;
using CsvHelper.Configuration;
using lab1.Application.Repositories;
using lab1.Domain.Entities;
using lab1.Infrastructure.Data.Csv;

namespace lab1.Infrastructure.Repositories;

public class CsvAirQualityProvider(lab1.Application.DTOs.FileOptions options) : ICsvAirQualityProvider
{
    public async Task<IReadOnlyList<AirQualityRecord>> ReadFromCsvAsync(Stream stream)
    {
        var config = new CsvConfiguration(options.GetCulture())
        {
            HasHeaderRecord = options.HasHeader,
            Delimiter = options.Delimiter,
            PrepareHeaderForMatch = args => args.Header.Trim().ToLowerInvariant()
        };

        using var reader = new StreamReader(stream, options.GetEncoding(), detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        using var csv = new CsvReader(reader, config);

        var list = new List<AirQualityRecord>();
        var id = 1;

        await foreach (var record in csv.GetRecordsAsync<AirQualityCsvRecord>())
        {
            var mapped = AirQualityMapper.Map(record, id++, options.GetCulture());
            list.Add(mapped);
        }

        return list;
    }

    public async Task<Stream> WriteToCsvAsync(List<AirQualityRecord> records)
    {
        var stream = new MemoryStream();

        await using var writer = new StreamWriter(
            stream,
            options.GetEncoding(),
            leaveOpen: true);

        await using var csv = new CsvWriter(writer, new CsvConfiguration(options.GetCulture()) { Delimiter = options.Delimiter, HasHeaderRecord = options.HasHeader });

        var csvRecords = records
            .Select(r => AirQualityMapper.ToCsvRecord(r, options.GetCulture()))
            .ToList();

        await csv.WriteRecordsAsync(csvRecords);
        await writer.FlushAsync();

        stream.Position = 0;
        return stream;
    }
}