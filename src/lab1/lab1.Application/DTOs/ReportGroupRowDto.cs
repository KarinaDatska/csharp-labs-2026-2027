namespace lab1.Application.DTOs;

public record ReportGroupRowDto(
    string Category,
    int RecordCount,
    double Percentage,
    double AveragePollution
);
