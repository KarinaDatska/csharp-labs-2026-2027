namespace lab1.Application.DTOs;

public class ReportMetadataDto
{
    public string StudentName { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string Variant { get; set; } = string.Empty;
    public string DatasetDescription { get; set; } =
        "Air Quality dataset containing PM2.5 measurements and monthly statistics for cities worldwide.";
}
