using System.Globalization;
using lab1.Domain.Entities;

namespace lab1.Infrastructure.Data.Csv;

public static class AirQualityMapper
{
    public static AirQualityCsvRecord ToCsvRecord(AirQualityRecord entity)
    {
        return new AirQualityCsvRecord
        {
            Rank = entity.Rank,
            City = entity.CityAndCountry,
            Avg = entity.AveragePollution,
            Jan = FormatDouble(entity.Measurements?.January),
            Feb = FormatDouble(entity.Measurements?.February),
            Mar = FormatDouble(entity.Measurements?.March),
            Apr = FormatDouble(entity.Measurements?.April),
            May = FormatDouble(entity.Measurements?.May),
            Jun = FormatDouble(entity.Measurements?.June),
            Jul = FormatDouble(entity.Measurements?.July),
            Aug = FormatDouble(entity.Measurements?.August),
            Sep = FormatDouble(entity.Measurements?.September),
            Oct = FormatDouble(entity.Measurements?.October),
            Nov = FormatDouble(entity.Measurements?.November),
            Dec = FormatDouble(entity.Measurements?.December)
        };
    }

    public static AirQualityRecord Map(AirQualityCsvRecord record, int id)
    {
        var measurements = new MonthlyMeasurements(
            january: ParseDouble(record.Jan),
            february: ParseDouble(record.Feb),
            march: ParseDouble(record.Mar),
            april: ParseDouble(record.Apr),
            may: ParseDouble(record.May),
            june: ParseDouble(record.Jun),
            july: ParseDouble(record.Jul),
            august: ParseDouble(record.Aug),
            september: ParseDouble(record.Sep),
            october: ParseDouble(record.Oct),
            november: ParseDouble(record.Nov),
            december: ParseDouble(record.Dec)
        );

        return new AirQualityRecord(
            id: id,
            rank: record.Rank,
            cityAndCountry: record.City,
            averagePollution: record.Avg,
            measurements: measurements
        );
    }

    private static double? ParseDouble(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim() == "-")
        {
            return null;
        }

        if (double.TryParse(value.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        return null;
    }

    private static string FormatDouble(double? value)
    {
        if (!value.HasValue) return "-";
        return value.Value.ToString(CultureInfo.InvariantCulture);
    }
}