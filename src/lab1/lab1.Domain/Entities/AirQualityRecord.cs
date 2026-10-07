namespace lab1.Domain.Entities;

public class AirQualityRecord
{
    public int Id { get; set; }
    public int Rank { get; set; }
    public string CityAndCountry { get; set; } = string.Empty;
    public double AveragePollution { get; set; }

    // Вкладена сутність для щомісячних вимірів
    public MonthlyMeasurements Measurements { get; set; } = new();

    public AirQualityRecord()
    {
    }

    public AirQualityRecord(
        int id,
        int rank,
        string cityAndCountry,
        double averagePollution,
        MonthlyMeasurements? measurements = null)
    {
        Id = id;
        Rank = rank;
        CityAndCountry = cityAndCountry;
        AveragePollution = averagePollution;
        Measurements = measurements ?? new MonthlyMeasurements();
    }
}