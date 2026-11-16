using lab1.Domain.Entities;
using lab1.Domain.Enums;

namespace lab1.Application.Repositories;

public interface IAirQualityDataStore
{
    IReadOnlyList<AirQualityRecord> Records { get; }
    DataFormat SourceFormat { get; }
    void SetData(List<AirQualityRecord> records, DataFormat sourceFormat);
    void Clear();
}
