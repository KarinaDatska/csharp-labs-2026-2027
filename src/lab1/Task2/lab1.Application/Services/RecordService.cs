using lab1.Application.DTOs;
using lab1.Application.Repositories;
using lab1.Domain.Entities;
using lab1.Domain.Validation;

namespace lab1.Application.Services;

public class RecordService(IAirQualityDataStore store, IDataAnalyticsService analytics)
{
    private static readonly object Gate = new();

    public object Query(AirQualityFilter filter, string sortBy, bool descending, int page, int pageSize)
    {
        if (page < 1 || pageSize < 1 || pageSize > 500) throw new ArgumentException("Некоректна сторінка.");
        var rows = analytics.FilterRecords(filter).ToList();
        var sorted = sortBy switch
        {
            "CityAndCountry" => descending ? rows.OrderByDescending(r => r.CityAndCountry) : rows.OrderBy(r => r.CityAndCountry),
            "AveragePollution" => descending ? rows.OrderByDescending(r => r.AveragePollution) : rows.OrderBy(r => r.AveragePollution),
            _ => descending ? rows.OrderByDescending(r => r.Rank) : rows.OrderBy(r => r.Rank)
        };
        var groups = rows.GroupBy(r => r.CityAndCountry.Split(',').Last().Trim())
            .Select(g => new { country = g.Key, count = g.Count(), sum = g.Sum(r => r.AveragePollution),
                average = g.Average(r => r.AveragePollution), min = g.Min(r => r.AveragePollution), max = g.Max(r => r.AveragePollution) });
        return new { total = rows.Count, records = sorted.Skip((page - 1) * pageSize).Take(pageSize),
            summary = ReportService.BuildReportData(rows), groups };
    }

    public AirQualityRecord Save(AirQualityRecord record, int? id)
    {
        RecordValidator.Validate(record);
        lock (Gate)
        {
            var rows = store.Records.ToList();
            if (id.HasValue)
            {
                var index = rows.FindIndex(r => r.Id == id);
                if (index < 0) throw new KeyNotFoundException("Запис не знайдено.");
                record.Id = id.Value;
                rows[index] = record;
            }
            else
            {
                record.Id = rows.Count == 0 ? 1 : rows.Max(r => r.Id) + 1;
                rows.Add(record);
            }
            store.SetData(rows, store.SourceFormat);
        }
        return record;
    }

    public void Delete(int id)
    {
        lock (Gate)
        {
            var rows = store.Records.ToList();
            if (rows.RemoveAll(r => r.Id == id) == 0) throw new KeyNotFoundException("Запис не знайдено.");
            store.SetData(rows, store.SourceFormat);
        }
    }
}
