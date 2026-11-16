using ClosedXML.Excel;
using lab1.Application.Repositories;
using lab1.Domain.Entities;

namespace lab1.Infrastructure.Repositories;

public class XlsxDataProvider(lab1.Application.DTOs.FileOptions options) : IXlsxAirQualityProvider
{
    public Task<IReadOnlyList<AirQualityRecord>> ReadFromXlsxAsync(Stream stream)
    {
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.FirstOrDefault(w => w.Name == options.Sheet) ?? workbook.Worksheet(1);
        var rows = worksheet.RowsUsed();

        var records = new List<AirQualityRecord>();
        var isHeader = options.HasHeader;
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
                Rank = row.Cell(1).TryGetValue<int>(out var r) ? r : throw new FormatException($"Некоректний ранг у рядку {row.RowNumber()}."),
                CityAndCountry = row.Cell(2).GetString(),
                AveragePollution = ParseRequired(row.Cell(3)),
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
        var worksheet = workbook.AddWorksheet(options.Sheet);

        var headers = new[]
        {
            "Rank", "City", "Avg", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        };

        for (var col = 0; options.HasHeader && col < headers.Length; col++)
        {
            worksheet.Cell(1, col + 1).Value = headers[col];
        }

        for (var i = 0; i < records.Count; i++)
        {
            var row = i + (options.HasHeader ? 2 : 1);
            var r = records[i];
            worksheet.Cell(row, 1).Value = r.Rank;
            worksheet.Cell(row, 2).Value = r.CityAndCountry;
            worksheet.Cell(row, 3).Value = r.AveragePollution;
            if (r.Measurements?.January is double value4) worksheet.Cell(row, 4).Value = value4;
            if (r.Measurements?.February is double value5) worksheet.Cell(row, 5).Value = value5;
            if (r.Measurements?.March is double value6) worksheet.Cell(row, 6).Value = value6;
            if (r.Measurements?.April is double value7) worksheet.Cell(row, 7).Value = value7;
            if (r.Measurements?.May is double value8) worksheet.Cell(row, 8).Value = value8;
            if (r.Measurements?.June is double value9) worksheet.Cell(row, 9).Value = value9;
            if (r.Measurements?.July is double value10) worksheet.Cell(row, 10).Value = value10;
            if (r.Measurements?.August is double value11) worksheet.Cell(row, 11).Value = value11;
            if (r.Measurements?.September is double value12) worksheet.Cell(row, 12).Value = value12;
            if (r.Measurements?.October is double value13) worksheet.Cell(row, 13).Value = value13;
            if (r.Measurements?.November is double value14) worksheet.Cell(row, 14).Value = value14;
            if (r.Measurements?.December is double value15) worksheet.Cell(row, 15).Value = value15;
        }

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return Task.FromResult<Stream>(stream);
    }

    private double ParseRequired(IXLCell cell) => TryParseNullableDouble(cell)
        ?? throw new FormatException($"Відсутнє середнє значення в {cell.Address}.");

    private double? TryParseNullableDouble(IXLCell cell)
    {
        if (cell.IsEmpty()) return null;
        if (cell.TryGetValue<double>(out var val)) return val;
        var str = cell.GetString();
        if (string.IsNullOrWhiteSpace(str) || str == "-") return null;
        if (double.TryParse(str, System.Globalization.NumberStyles.Float, options.GetCulture(), out var d)) return d;
        throw new FormatException($"Некоректне числове значення: {str}");
    }
}
