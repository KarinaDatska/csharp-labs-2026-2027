using CsvHelper.Configuration.Attributes;

namespace lab1.Infrastructure.Data.Csv;

public class AirQualityCsvRecord
{
    [Name("rank")]
    public int Rank { get; set; }

    [Name("city")]
    public string City { get; set; } = string.Empty;

    [Name("avg")]
    public double Avg { get; set; }

    [Name("jan")]
    public string Jan { get; set; } = string.Empty;

    [Name("feb")]
    public string Feb { get; set; } = string.Empty;

    [Name("mar")]
    public string Mar { get; set; } = string.Empty;

    [Name("apr")]
    public string Apr { get; set; } = string.Empty;

    [Name("may")]
    public string May { get; set; } = string.Empty;

    [Name("jun")]
    public string Jun { get; set; } = string.Empty;

    [Name("jul")]
    public string Jul { get; set; } = string.Empty;

    [Name("aug")]
    public string Aug { get; set; } = string.Empty;

    [Name("sep")]
    public string Sep { get; set; } = string.Empty;

    [Name("oct")]
    public string Oct { get; set; } = string.Empty;

    [Name("nov")]
    public string Nov { get; set; } = string.Empty;

    [Name("dec")]
    public string Dec { get; set; } = string.Empty;
}
