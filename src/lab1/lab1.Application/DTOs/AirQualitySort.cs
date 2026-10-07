using lab1.Domain.Enums;

namespace lab1.Application.DTOs;

public class AirQualitySort
{
    public AirQualitySortBy SortBy { get; set; } = AirQualitySortBy.Rank;
    public SortDirection Direction { get; set; } = SortDirection.Ascending;
}