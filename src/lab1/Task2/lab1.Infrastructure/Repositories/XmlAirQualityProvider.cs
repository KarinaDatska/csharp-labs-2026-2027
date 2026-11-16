using System.Text;
using System.Xml.Serialization;
using System.Xml;
using lab1.Application.Repositories;
using lab1.Domain.Entities;

namespace lab1.Infrastructure.Repositories;

public class XmlAirQualityProvider(lab1.Application.DTOs.FileOptions options) : IXmlAirQualityProvider
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
        using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = options.GetEncoding(), Indent = true, CloseOutput = false }))
            Serializer.Serialize(writer, records);
        stream.Position = 0;
        return Task.FromResult<Stream>(stream);
    }
}
