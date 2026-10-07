using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using lab1.Application.Repositories;
using lab1.Domain.Entities;
using lab1.Infrastructure.Data.Csv;

namespace lab1.Infrastructure.Repositories;

public class CsvAirQualityProvider : ICsvAirQualityProvider
{
    public async Task<IReadOnlyList<AirQualityRecord>> ReadFromCsvAsync(Stream stream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ",",
            BadDataFound = null,
            MissingFieldFound = null
        };

        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        using var csv = new CsvReader(reader, config);

        var list = new List<AirQualityRecord>();
        var id = 1;

        await foreach (var record in csv.GetRecordsAsync<AirQualityCsvRecord>())
        {
            list.Add(AirQualityMapper.Map(record, id++));
        }

        return list;
    }

    public async Task<Stream> WriteToCsvAsync(List<AirQualityRecord> records)
    {
        var stream = new MemoryStream();

        await using var writer = new StreamWriter(
            stream,
            new UTF8Encoding(false),
            leaveOpen: true);

        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        var csvRecords = records
            .Select(AirQualityMapper.ToCsvRecord)
            .ToList();

        await csv.WriteRecordsAsync(csvRecords);
        await writer.FlushAsync();

        stream.Position = 0;
        return stream;
    }
}