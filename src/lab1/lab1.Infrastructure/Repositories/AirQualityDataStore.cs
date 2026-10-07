using lab1.Application.Repositories;
using lab1.Domain.Entities;
using lab1.Domain.Enums;

namespace lab1.Infrastructure.Repositories;

public class AirQualityDataStore : IAirQualityDataStore
{
    private List<AirQualityRecord> _records = [];

    public IReadOnlyList<AirQualityRecord> Records => _records;

    public DataFormat SourceFormat { get; private set; } = DataFormat.Null;

    public void SetData(List<AirQualityRecord> records, DataFormat sourceFormat)
    {
        _records = records ?? [];
        SourceFormat = sourceFormat;
    }

    public void Clear()
    {
        _records.Clear();
        SourceFormat = DataFormat.Null;
    }
}