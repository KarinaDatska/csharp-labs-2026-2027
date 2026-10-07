using lab1.Application.DTOs;
using lab1.Domain.Entities;

namespace lab1.Application.Services;

public interface IDataAnalyticsService
{
    IEnumerable<AirQualityRecord> GetAllRecords();
    AirQualityRecord? GetRecordById(int id);
    AirQualityRecord? FindByCity(string cityName);
    IEnumerable<AirQualityRecord> FilterRecords(AirQualityFilter filter);
    IEnumerable<AirQualityRecord> SortRecords(AirQualitySort sort);
    double GetSum();
    double GetMedian();
    AirQualityRecord? GetMin();
    AirQualityRecord? GetMax();
    int GetTotalCount();
}
