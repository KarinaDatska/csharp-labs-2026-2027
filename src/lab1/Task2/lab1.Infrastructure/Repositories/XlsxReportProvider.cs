using ClosedXML.Excel;
using lab1.Application.DTOs;
using lab1.Application.Repositories;
using lab1.Domain.Entities;

namespace lab1.Infrastructure.Repositories;

public class XlsxReportProvider : IXlsxReportProvider
{
    public Task<Stream> CreateReportAsync(
        IReadOnlyList<AirQualityRecord> records,
        ReportDataDto reportData)
    {
        using var workbook = new XLWorkbook();

        CreateDataWorksheet(workbook, records);
        CreateSummaryWorksheet(workbook, reportData);

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        lab1.Infrastructure.Reports.XlsxCharts.Add(stream, reportData);
        stream.Position = 0;

        return Task.FromResult<Stream>(stream);
    }

    private static void CreateDataWorksheet(XLWorkbook workbook, IReadOnlyList<AirQualityRecord> records)
    {
        var ws = workbook.AddWorksheet("Air Quality Records");

        var headers = new[]
        {
            "Rank", "City and Country", "Average PM2.5", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        };

        for (var col = 0; col < headers.Length; col++)
        {
            var cell = ws.Cell(1, col + 1);
            cell.Value = headers[col];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78");
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        for (var i = 0; i < records.Count; i++)
        {
            var row = i + 2;
            var r = records[i];
            ws.Cell(row, 1).Value = r.Rank;
            ws.Cell(row, 2).Value = r.CityAndCountry;
            ws.Cell(row, 3).Value = r.AveragePollution;
            if (r.Measurements?.January is double value4) ws.Cell(row, 4).Value = value4;
            if (r.Measurements?.February is double value5) ws.Cell(row, 5).Value = value5;
            if (r.Measurements?.March is double value6) ws.Cell(row, 6).Value = value6;
            if (r.Measurements?.April is double value7) ws.Cell(row, 7).Value = value7;
            if (r.Measurements?.May is double value8) ws.Cell(row, 8).Value = value8;
            if (r.Measurements?.June is double value9) ws.Cell(row, 9).Value = value9;
            if (r.Measurements?.July is double value10) ws.Cell(row, 10).Value = value10;
            if (r.Measurements?.August is double value11) ws.Cell(row, 11).Value = value11;
            if (r.Measurements?.September is double value12) ws.Cell(row, 12).Value = value12;
            if (r.Measurements?.October is double value13) ws.Cell(row, 13).Value = value13;
            if (r.Measurements?.November is double value14) ws.Cell(row, 14).Value = value14;
            if (r.Measurements?.December is double value15) ws.Cell(row, 15).Value = value15;

            if (i % 2 == 1)
            {
                ws.Range(row, 1, row, headers.Length).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F5F9");
            }
        }

        ws.Columns().AdjustToContents();
        if (records.Count > 0)
            ws.Range(2, 3, records.Count + 1, 3).AddConditionalFormat().ColorScale()
                .LowestValue(XLColor.LightGreen).HighestValue(XLColor.LightPink);
    }

    private static void CreateSummaryWorksheet(XLWorkbook workbook, ReportDataDto reportData)
    {
        var ws = workbook.AddWorksheet("Summary");

        ws.Cell("A1").Value = "ЗВЕДЕНИЙ ЗВІТ З ЯКОСТІ ПОВІТРЯ";
        ws.Cell("A1").Style.Font.Bold = true;
        ws.Cell("A1").Style.Font.FontSize = 14;
        ws.Cell("A1").Style.Font.FontColor = XLColor.FromHtml("#1F4E78");

        ws.Cell("A3").Value = "Показник";
        ws.Cell("B3").Value = "Значення";
        ws.Range("A3:B3").Style.Font.Bold = true;
        ws.Range("A3:B3").Style.Fill.BackgroundColor = XLColor.FromHtml("#D9EAF7");

        ws.Cell("A4").Value = "Всього записів (міст)";
        ws.Cell("B4").Value = reportData.TotalRecords;

        ws.Cell("A5").Value = "Середнє значення PM2.5";
        ws.Cell("B5").Value = reportData.AveragePollutionMean;

        ws.Cell("A6").Value = "Медіана PM2.5";
        ws.Cell("B6").Value = reportData.MedianPollution;

        ws.Cell("A7").Value = "Мінімальне значення PM2.5";
        ws.Cell("B7").Value = reportData.MinPollution;

        ws.Cell("A8").Value = "Максимальне значення PM2.5";
        ws.Cell("B8").Value = reportData.MaxPollution;

        // Розподіл за категоріями
        ws.Cell("A10").Value = "Розподіл за категоріями якості";
        ws.Cell("A10").Style.Font.Bold = true;

        ws.Cell("A11").Value = "Категорія";
        ws.Cell("B11").Value = "Кількість";
        ws.Cell("C11").Value = "Відсоток (%)";
        ws.Cell("D11").Value = "Середнє PM2.5";
        ws.Range("A11:D11").Style.Font.Bold = true;
        ws.Range("A11:D11").Style.Fill.BackgroundColor = XLColor.FromHtml("#D9EAF7");

        var rowIndex = 12;
        foreach (var cat in reportData.ByCategory)
        {
            ws.Cell(rowIndex, 1).Value = cat.Category;
            ws.Cell(rowIndex, 2).Value = cat.RecordCount;
            ws.Cell(rowIndex, 3).Value = $"{cat.Percentage}%";
            ws.Cell(rowIndex, 4).Value = cat.AveragePollution;
            rowIndex++;
        }

        // Сезонна динаміка по місяцях
        rowIndex += 1;
        ws.Cell(rowIndex, 1).Value = "Середній рівень по місяцях";
        ws.Cell(rowIndex, 1).Style.Font.Bold = true;

        rowIndex++;
        ws.Cell(rowIndex, 1).Value = "Місяць";
        ws.Cell(rowIndex, 2).Value = "Середнє PM2.5";
        ws.Range(rowIndex, 1, rowIndex, 2).Style.Font.Bold = true;
        ws.Range(rowIndex, 1, rowIndex, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#D9EAF7");

        rowIndex++;
        foreach (var m in reportData.MonthlyAverages)
        {
            ws.Cell(rowIndex, 1).Value = m.Month;
            ws.Cell(rowIndex, 2).Value = m.AveragePollution;
            rowIndex++;
        }

        ws.Columns().AdjustToContents();
    }
}
