using lab1.Application.DTOs;
using lab1.Application.Repositories;
using lab1.Domain.Entities;
using lab1.Domain.Enums;

namespace lab1.Application.Services;

public class XlsxAirQualityService : IXlsxAirQualityService
{
    private readonly IXlsxAirQualityProvider _xlsxDataProvider;
    private readonly IAirQualityDataStore _dataStore;

    public XlsxAirQualityService(
        IXlsxAirQualityProvider xlsxDataProvider,
        IAirQualityDataStore dataStore)
    {
        _xlsxDataProvider = xlsxDataProvider;
        _dataStore = dataStore;
    }

    public async Task<IReadOnlyList<AirQualityDto>> ReadFromXlsxAsync(Stream stream)
    {
        var records = await _xlsxDataProvider.ReadFromXlsxAsync(stream);

        _dataStore.SetData(records.ToList(), DataFormat.Xlsx);

        return records.Select(CsvAirQualityService.MapToDto).ToList();
    }

    public Task<Stream> WriteToXlsxAsync()
    {
        var records = _dataStore.Records.ToList();
        return _xlsxDataProvider.WriteToXlsxAsync(records);
    }
}