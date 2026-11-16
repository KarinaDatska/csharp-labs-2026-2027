using System.Globalization;
using lab1.Domain.Entities;

namespace lab1.Infrastructure.Data.Csv;

public static class AirQualityMapper
{
    public static AirQualityCsvRecord ToCsvRecord(AirQualityRecord entity, CultureInfo? culture = null)
    {
        return new AirQualityCsvRecord
        {
            Rank = entity.Rank,
            City = entity.CityAndCountry,
            Avg = entity.AveragePollution,
            Jan = FormatDouble(entity.Measurements?.January, culture),
            Feb = FormatDouble(entity.Measurements?.February, culture),
            Mar = FormatDouble(entity.Measurements?.March, culture),
            Apr = FormatDouble(entity.Measurements?.April, culture),
            May = FormatDouble(entity.Measurements?.May, culture),
            Jun = FormatDouble(entity.Measurements?.June, culture),
            Jul = FormatDouble(entity.Measurements?.July, culture),
            Aug = FormatDouble(entity.Measurements?.August, culture),
            Sep = FormatDouble(entity.Measurements?.September, culture),
            Oct = FormatDouble(entity.Measurements?.October, culture),
            Nov = FormatDouble(entity.Measurements?.November, culture),
            Dec = FormatDouble(entity.Measurements?.December, culture)
        };
    }

    public static AirQualityRecord Map(AirQualityCsvRecord record, int id, CultureInfo? culture = null)
    {
        var measurements = new MonthlyMeasurements(
            january: ParseDouble(record.Jan, culture),
            february: ParseDouble(record.Feb, culture),
            march: ParseDouble(record.Mar, culture),
            april: ParseDouble(record.Apr, culture),
            may: ParseDouble(record.May, culture),
            june: ParseDouble(record.Jun, culture),
            july: ParseDouble(record.Jul, culture),
            august: ParseDouble(record.Aug, culture),
            september: ParseDouble(record.Sep, culture),
            october: ParseDouble(record.Oct, culture),
            november: ParseDouble(record.Nov, culture),
            december: ParseDouble(record.Dec, culture)
        );

        return new AirQualityRecord(
            id: id,
            rank: record.Rank,
            cityAndCountry: record.City,
            averagePollution: record.Avg,
            measurements: measurements
        );
    }

    private static double? ParseDouble(string? value, CultureInfo? culture)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim() == "-")
        {
            return null;
        }

        if (double.TryParse(value.Trim(), NumberStyles.Float, culture ?? CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        throw new FormatException($"Некоректне місячне значення: {value}");
    }

    private static string FormatDouble(double? value, CultureInfo? culture)
    {
        if (!value.HasValue) return "-";
        return value.Value.ToString(culture ?? CultureInfo.InvariantCulture);
    }
}