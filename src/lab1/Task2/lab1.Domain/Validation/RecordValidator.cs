using lab1.Domain.Entities;

namespace lab1.Domain.Validation;

public static class RecordValidator
{
    public static void Validate(AirQualityRecord record)
    {
        if (record.Rank < 1 || string.IsNullOrWhiteSpace(record.CityAndCountry))
            throw new ArgumentException("Ранг має бути додатним, місто — заповненим.");
        Check(record.AveragePollution);
        if (record.Measurements == null) throw new ArgumentException("Відсутні місячні вимірювання.");
        foreach (var property in typeof(MonthlyMeasurements).GetProperties())
            if (property.GetValue(record.Measurements) is double value) Check(value);
    }

    private static void Check(double value)
    {
        if (!double.IsFinite(value) || value < 0)
            throw new ArgumentException("Забруднення має бути невід’ємним скінченним числом.");
    }
}
