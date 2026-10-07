using lab1.Application.DTOs;
using lab1.Application.Repositories;
using lab1.Domain.Entities;
using lab1.Domain.Enums;

namespace lab1.Application.Services;

public class DataAnalyticsService : IDataAnalyticsService
{
    private readonly IAirQualityDataStore _dataStore;

    public DataAnalyticsService(IAirQualityDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public double GetSum()
    {
        return _dataStore.Records.Sum(x => x.AveragePollution);
    }

    public double GetMedian()
    {
        var values = _dataStore.Records
            .Select(x => x.AveragePollution)
            .OrderBy(x => x)
            .ToList();

        if (values.Count == 0)
        {
            return 0;
        }

        var middle = values.Count / 2;

        if (values.Count % 2 == 0)
        {
            return (values[middle - 1] + values[middle]) / 2.0;
        }

        return values[middle];
    }

    public AirQualityRecord? GetMin()
    {
        return _dataStore.Records.MinBy(x => x.AveragePollution);
    }

    public AirQualityRecord? GetMax()
    {
        return _dataStore.Records.MaxBy(x => x.AveragePollution);
    }

    public int GetTotalCount()
    {
        return _dataStore.Records.Count;
    }

    public IEnumerable<AirQualityRecord> GetAllRecords()
    {
        return _dataStore.Records;
    }

    public AirQualityRecord? GetRecordById(int id)
    {
        return _dataStore.Records.FirstOrDefault(x => x.Id == id);
    }

    public AirQualityRecord? FindByCity(string cityName)
    {
        if (string.IsNullOrWhiteSpace(cityName)) return null;

        return _dataStore.Records.FirstOrDefault(x =>
            !string.IsNullOrEmpty(x.CityAndCountry) &&
            x.CityAndCountry.Contains(cityName, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<AirQualityRecord> FilterRecords(AirQualityFilter filter)
    {
        IEnumerable<AirQualityRecord> query = _dataStore.Records;

        if (!string.IsNullOrWhiteSpace(filter.City))
        {
            query = query.Where(r => !string.IsNullOrEmpty(r.CityAndCountry) &&
                                     r.CityAndCountry.Contains(filter.City, StringComparison.OrdinalIgnoreCase));
        }

        if (filter.MinRank.HasValue)
        {
            query = query.Where(r => r.Rank >= filter.MinRank.Value);
        }

        if (filter.MaxRank.HasValue)
        {
            query = query.Where(r => r.Rank <= filter.MaxRank.Value);
        }

        if (filter.MinAveragePollution.HasValue)
        {
            query = query.Where(r => r.AveragePollution >= filter.MinAveragePollution.Value);
        }

        if (filter.MaxAveragePollution.HasValue)
        {
            query = query.Where(r => r.AveragePollution <= filter.MaxAveragePollution.Value);
        }

        return query;
    }

    public IEnumerable<AirQualityRecord> SortRecords(AirQualitySort sort)
    {
        IEnumerable<AirQualityRecord> query = _dataStore.Records;

        return sort.SortBy switch
        {
            AirQualitySortBy.Rank =>
                sort.Direction == SortDirection.Ascending
                    ? query.OrderBy(r => r.Rank)
                    : query.OrderByDescending(r => r.Rank),

            AirQualitySortBy.CityAndCountry =>
                sort.Direction == SortDirection.Ascending
                    ? query.OrderBy(r => r.CityAndCountry)
                    : query.OrderByDescending(r => r.CityAndCountry),

            AirQualitySortBy.AveragePollution =>
                sort.Direction == SortDirection.Ascending
                    ? query.OrderBy(r => r.AveragePollution)
                    : query.OrderByDescending(r => r.AveragePollution),

            _ => query
        };
    }
}
