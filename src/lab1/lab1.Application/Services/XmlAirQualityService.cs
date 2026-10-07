using lab1.Application.DTOs;
using lab1.Application.Repositories;
using lab1.Domain.Entities;
using lab1.Domain.Enums;

namespace lab1.Application.Services;

public class XmlAirQualityService : IXmlAirQualityService
{
    private readonly IXmlAirQualityProvider _xmlDataProvider;
    private readonly IAirQualityDataStore _dataStore;

    public XmlAirQualityService(
        IXmlAirQualityProvider xmlDataProvider,
        IAirQualityDataStore dataStore)
    {
        _xmlDataProvider = xmlDataProvider;
        _dataStore = dataStore;
    }

    public async Task<IReadOnlyList<AirQualityDto>> ReadFromXmlAsync(Stream stream)
    {
        var records = await _xmlDataProvider.ReadFromXmlAsync(stream);

        _dataStore.SetData(records.ToList(), DataFormat.Xml);

        return records.Select(CsvAirQualityService.MapToDto).ToList();
    }

    public Task<Stream> WriteToXmlAsync()
    {
        var records = _dataStore.Records.ToList();
        return _xmlDataProvider.WriteToXmlAsync(records);
    }
}