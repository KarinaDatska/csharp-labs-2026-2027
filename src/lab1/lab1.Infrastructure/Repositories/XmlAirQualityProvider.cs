using System.Text;
using System.Xml.Serialization;
using lab1.Application.Repositories;
using lab1.Domain.Entities;

namespace lab1.Infrastructure.Repositories;

public class XmlAirQualityProvider : IXmlAirQualityProvider
{
    private static readonly XmlSerializer Serializer = new(typeof(List<AirQualityRecord>));

    public Task<IReadOnlyList<AirQualityRecord>> ReadFromXmlAsync(Stream stream)
    {
        var records = (List<AirQualityRecord>?)Serializer.Deserialize(stream);
        return Task.FromResult<IReadOnlyList<AirQualityRecord>>(records ?? []);
    }

    public Task<Stream> WriteToXmlAsync(List<AirQualityRecord> records)
    {
        var stream = new MemoryStream();
        Serializer.Serialize(stream, records);
        stream.Position = 0;
        return Task.FromResult<Stream>(stream);
    }
}