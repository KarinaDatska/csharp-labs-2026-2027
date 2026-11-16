using lab1.Application.DTOs;
using lab1.Application.Repositories;
using lab1.Domain.Entities;
using lab1.Domain.Enums;

namespace lab1.Application.Services;

public class CsvAirQualityService : ICsvAirQualityService
{
    private readonly ICsvAirQualityProvider _csvProvider;
    private readonly IAirQualityDataStore _dataStore;

    public CsvAirQualityService(
        ICsvAirQualityProvider csvProvider,
        IAirQualityDataStore dataStore)
    {
        _csvProvider = csvProvider;
        _dataStore = dataStore;
    }

    public async Task<IReadOnlyList<AirQualityDto>> ReadFromCsvAsync(Stream stream, string sourceFilePath)
    {
        var records = await _csvProvider.ReadFromCsvAsync(stream);

        _dataStore.SetData(records.ToList(), DataFormat.Csv);

        return records.Select(MapToDto).ToList();
    }

    public async Task<Stream> WriteToCsvAsync()
    {
        var records = _dataStore.Records.ToList();
        return await _csvProvider.WriteToCsvAsync(records);
    }

    public static AirQualityDto MapToDto(AirQualityRecord r)
    {
        return new AirQualityDto
        {
            Id = r.Id,
            Rank = r.Rank,
            CityAndCountry = r.CityAndCountry,
            AveragePollution = r.AveragePollution,
            Measurements = new MonthlyMeasurementsDto
            {
                January = r.Measurements?.January,
                February = r.Measurements?.February,
                March = r.Measurements?.March,
                April = r.Measurements?.April,
                May = r.Measurements?.May,
                June = r.Measurements?.June,
                July = r.Measurements?.July,
                August = r.Measurements?.August,
                September = r.Measurements?.September,
                October = r.Measurements?.October,
                November = r.Measurements?.November,
                December = r.Measurements?.December
            }
        };
    }
}