using ClosedXML.Excel;
using lab1.Application.Repositories;
using lab1.Domain.Entities;

namespace lab1.Infrastructure.Repositories;

public class XlsxDataProvider : IXlsxAirQualityProvider
{
    public Task<IReadOnlyList<AirQualityRecord>> ReadFromXlsxAsync(Stream stream)
    {
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RowsUsed();

        var records = new List<AirQualityRecord>();
        var isHeader = true;
        var idCounter = 1;

        foreach (var row in rows)
        {
            if (isHeader)
            {
                isHeader = false;
                continue;
            }

            var record = new AirQualityRecord
            {
                Id = idCounter++,
                Rank = row.Cell(1).TryGetValue<int>(out var r) ? r : 0,
                CityAndCountry = row.Cell(2).GetString(),
                AveragePollution = row.Cell(3).TryGetValue<double>(out var avg) ? avg : 0,
                Measurements = new MonthlyMeasurements
                {
                    January = TryParseNullableDouble(row.Cell(4)),
                    February = TryParseNullableDouble(row.Cell(5)),
                    March = TryParseNullableDouble(row.Cell(6)),
                    April = TryParseNullableDouble(row.Cell(7)),
                    May = TryParseNullableDouble(row.Cell(8)),
                    June = TryParseNullableDouble(row.Cell(9)),
                    July = TryParseNullableDouble(row.Cell(10)),
                    August = TryParseNullableDouble(row.Cell(11)),
                    September = TryParseNullableDouble(row.Cell(12)),
                    October = TryParseNullableDouble(row.Cell(13)),
                    November = TryParseNullableDouble(row.Cell(14)),
                    December = TryParseNullableDouble(row.Cell(15))
                }
            };

            records.Add(record);
        }

        return Task.FromResult<IReadOnlyList<AirQualityRecord>>(records);
    }

    public Task<Stream> WriteToXlsxAsync(List<AirQualityRecord> records)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("AirQuality");

        var headers = new[]
        {
            "Rank", "City", "Avg", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        };

        for (var col = 0; col < headers.Length; col++)
        {
            worksheet.Cell(1, col + 1).Value = headers[col];
        }

        for (var i = 0; i < records.Count; i++)
        {
            var row = i + 2;
            var r = records[i];
            worksheet.Cell(row, 1).Value = r.Rank;
            worksheet.Cell(row, 2).Value = r.CityAndCountry;
            worksheet.Cell(row, 3).Value = r.AveragePollution;
            worksheet.Cell(row, 4).Value = r.Measurements?.January ?? 0;
            worksheet.Cell(row, 5).Value = r.Measurements?.February ?? 0;
            worksheet.Cell(row, 6).Value = r.Measurements?.March ?? 0;
            worksheet.Cell(row, 7).Value = r.Measurements?.April ?? 0;
            worksheet.Cell(row, 8).Value = r.Measurements?.May ?? 0;
            worksheet.Cell(row, 9).Value = r.Measurements?.June ?? 0;
            worksheet.Cell(row, 10).Value = r.Measurements?.July ?? 0;
            worksheet.Cell(row, 11).Value = r.Measurements?.August ?? 0;
            worksheet.Cell(row, 12).Value = r.Measurements?.September ?? 0;
            worksheet.Cell(row, 13).Value = r.Measurements?.October ?? 0;
            worksheet.Cell(row, 14).Value = r.Measurements?.November ?? 0;
            worksheet.Cell(row, 15).Value = r.Measurements?.December ?? 0;
        }

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return Task.FromResult<Stream>(stream);
    }

    private static double? TryParseNullableDouble(IXLCell cell)
    {
        if (cell.IsEmpty()) return null;
        if (cell.TryGetValue<double>(out var val)) return val;
        var str = cell.GetString();
        if (string.IsNullOrWhiteSpace(str) || str == "-") return null;
        if (double.TryParse(str, out var d)) return d;
        return null;
    }
}