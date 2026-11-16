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
        records ??= [];
        foreach (var record in records) lab1.Domain.Validation.RecordValidator.Validate(record);
        var used = new HashSet<int>();
        var nextId = 1;
        foreach (var record in records)
        {
            if (record.Id <= 0 || !used.Add(record.Id))
            {
                while (used.Contains(nextId)) nextId++;
                record.Id = nextId;
                used.Add(nextId++);
            }
        }
        _records = records;
        SourceFormat = sourceFormat;
    }

    public void Clear()
    {
        _records.Clear();
        SourceFormat = DataFormat.Null;
    }
}