using lab1.Application.DTOs;
using lab1.Application.Repositories;
using lab1.Domain.Entities;
using lab1.Domain.Enums;

namespace lab1.Application.Services;

public class JsonAirQualityService : IJsonAirQualityService
{
    private readonly IJsonAirQualityProvider _jsonDataProvider;
    private readonly IAirQualityDataStore _dataStore;

    public JsonAirQualityService(
        IJsonAirQualityProvider jsonDataProvider,
        IAirQualityDataStore dataStore)
    {
        _jsonDataProvider = jsonDataProvider;
        _dataStore = dataStore;
    }

    public async Task<IReadOnlyList<AirQualityDto>> ReadFromJsonAsync(Stream stream)
    {
        var records = await _jsonDataProvider.ReadFromJsonAsync(stream);

        _dataStore.SetData(records.ToList(), DataFormat.Json);

        return records.Select(CsvAirQualityService.MapToDto).ToList();
    }

    public Task<Stream> WriteToJsonAsync()
    {
        var records = _dataStore.Records.ToList();
        return _jsonDataProvider.WriteToJsonAsync(records);
    }
}