using System;
using System.Buffers.Binary;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using lab1.Application.DTOs;
using lab1.Application.Repositories;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace lab1.Infrastructure.Repositories;

// генератор DOCX. Діаграми мають бути у форматі png

public class DocxReportProvider : IDocxReportProvider
{
    private const string Font = "Times New Roman";
    private const string HeaderFill = "E7EDF4";
    private const int PageWidth = 11906;  // A4
    private const int PageHeight = 16838;
    private const int Margin = 1134;      // 20 mm
    private const int ContentWidth = PageWidth - 2 * Margin;
    private const long ImageMaxWidth = 5_760_000L;  // ~16 cm
    private const long ImageMaxHeight = 2_520_000L; // 7 cm
    private static readonly CultureInfo Ua = CultureInfo.GetCultureInfo("uk-UA");

    public Task<Stream> CreateReportAsync(
        ReportDataDto reportData,
        ReportMetadataDto metadata,
        byte[]? topPollutedChart,
        byte[]? distributionChart,
        byte[]? monthlyTrendChart)
    {
        ArgumentNullException.ThrowIfNull(reportData);
        ArgumentNullException.ThrowIfNull(metadata);

        var stream = new MemoryStream();
        try
        {
            using (var doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
            {
                var main = doc.AddMainDocumentPart();
                main.Document = new Document(new Body());
                var body = main.Document.Body!;

                AddTitle(body, metadata);
                AddDataset(body, metadata);
                if (!string.IsNullOrWhiteSpace(metadata.ScreenshotPng))
                {
                    var image = Convert.FromBase64String(metadata.ScreenshotPng);
                    if (image.Length > 5_000_000 || !image.Take(8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }))
                        throw new ArgumentException("Оберіть PNG-знімок інтерфейсу розміром до 5 МБ.");
                    AddImage(main, body, image, 100, "Інтерфейс застосунку");
                }
                AddStatistics(body, reportData);
                AddTables(body, reportData);
                AddCharts(main, body, topPollutedChart, distributionChart, monthlyTrendChart);
                body.Append(H("5. Висновки"));
                body.Append(P($"Опрацьовано {reportData.TotalRecords} записів. Середній показник забруднення становить {N(reportData.AveragePollutionMean)}, мінімальний — {N(reportData.MinPollution)}, максимальний — {N(reportData.MaxPollution)}.", indent: true));
                body.Append(P("Місячні середні дозволяють порівняти сезонні зміни. Пропущені вимірювання не включаються в обчислення середнього. Для пояснення причин відмінностей між містами потрібні додаткові дані.", indent: true));

                body.Append(new SectionProperties(
                    new PageSize { Width = (uint)PageWidth, Height = (uint)PageHeight },
                    new PageMargin { Top = Margin, Bottom = Margin, Left = (uint)Margin, Right = (uint)Margin }));
                main.Document.Save();
            }

            stream.Position = 0;
            return Task.FromResult<Stream>(stream);
        }
        catch
        {
            stream.Dispose();
            throw;
        }
    }

    private static void AddTitle(Body body, ReportMetadataDto meta)
    {
        body.Append(P("МІНІСТЕРСТВО ОСВІТИ І НАУКИ УКРАЇНИ", 12, center: true, after: 100));
        body.Append(P("НАЦІОНАЛЬНИЙ УНІВЕРСИТЕТ «ОСТРОЗЬКА АКАДЕМІЯ»", 12, bold: true, center: true));
        body.Append(P("ЗВІТ", 18, bold: true, center: true, before: 1500));
        body.Append(P("до лабораторної роботи №1", 14, center: true));
        body.Append(P("Аналіз та візуалізація даних якості повітря", 14, bold: true, center: true));
        body.Append(P("Дисципліна: Програмування мовою C# (.NET)", 12, center: true, after: 700));

        var widths = new[] { 2300, ContentWidth - 2300 };
        var table = MakeTable(widths, borders: false);
        AddRow(table, widths, new[] { "Студент:", Or(meta.StudentName, "Студент") });
        AddRow(table, widths, new[] { "Група:", Or(meta.Group, "КН") });
        AddRow(table, widths, new[] { "Варіант:", Or(meta.Variant, "1") });
        body.Append(table);
        body.Append(P("Острог — 2026", 12, center: true, before: 1000));
    }

    private static void AddDataset(Body body, ReportMetadataDto meta)
    {
        body.Append(H("1. Опис предметної області та набору даних", newPage: true));
        body.Append(P("Предметна область цієї лабораторної роботи — аналіз якості повітря (PM2.5) у містах світу. " +
                      "Набір даних містить місто, країну, рейтинг, середньорічну концентрацію та помісячні виміри.", indent: true));
        body.Append(P("Характеристика набору даних: " + Or(meta.DatasetDescription, "не вказано"), indent: true));
        body.Append(P("Основні атрибути набору даних:", bold: true));
        body.Append(Bullet("Rank — порядковий номер міста;"));
        body.Append(Bullet("CityAndCountry — назва міста та країна;"));
        body.Append(Bullet("AveragePollution — середньорічна концентрація PM2.5 (мкг/м³);"));
        body.Append(Bullet("MonthlyMeasurements — концентрація за місяцями (January–December)."));
    }

    private static void AddStatistics(Body body, ReportDataDto data)
    {
        body.Append(H("2. Загальні статистичні показники"));
        body.Append(P("Для завантаженого набору даних обчислено основні статистичні показники.", indent: true));
        var widths = new[] { 6300, ContentWidth - 6300 };
        var table = MakeTable(widths);
        AddRow(table, widths, new[] { "Метрика", "Значення" }, header: true);
        AddRow(table, widths, new[] { "Кількість міст", data.TotalRecords.ToString(Ua) });
        AddRow(table, widths, new[] { "Середній рівень PM2.5", N(data.AveragePollutionMean) });
        AddRow(table, widths, new[] { "Медіана PM2.5", N(data.MedianPollution) });
        AddRow(table, widths, new[] { "Мінімальний рівень PM2.5", N(data.MinPollution) });
        AddRow(table, widths, new[] { "Максимальний рівень PM2.5", N(data.MaxPollution) });
        body.Append(table);
    }

    private static void AddTables(Body body, ReportDataDto data)
    {
        body.Append(H("3. Зведені аналітичні таблиці", newPage: true));
        body.Append(H("3.1. Розподіл міст за категоріями безпеки повітря", level: 2));
        var widths = new[] { 3300, 1900, 1500, ContentWidth - 6700 };
        var table = MakeTable(widths);
        AddRow(table, widths, new[] { "Категорія", "Кількість", "Частка (%)", "Середнє PM2.5" }, header: true);
        foreach (var item in data.ByCategory)
            AddRow(table, widths, new[] { item.Category ?? "", item.RecordCount.ToString(Ua),
                                         N(item.Percentage) + "%", N(item.AveragePollution) });
        body.Append(table);

        body.Append(H("3.2. Сезонні коливання за місяцями", level: 2));
        widths = new[] { 4000, ContentWidth - 4000 };
        table = MakeTable(widths);
        AddRow(table, widths, new[] { "Місяць", "Середній рівень PM2.5" }, header: true);
        foreach (var item in data.MonthlyAverages)
            AddRow(table, widths, new[] { item.Month ?? "", N(item.AveragePollution) });
        body.Append(table);
    }

    private static void AddCharts(MainDocumentPart main, Body body, byte[]? top, byte[]? distribution, byte[]? monthly)
    {
        body.Append(H("4. Графічна візуалізація", newPage: true));
        uint id = 1;
        if (top is not null) AddImage(main, body, top, id++, "Топ-10 найбільш забруднених міст світу");
        if (distribution is not null) AddImage(main, body, distribution, id++, "Розподіл міст за категоріями якості повітря");
        if (monthly is not null) AddImage(main, body, monthly, id++, "Сезонна динаміка забруднення повітря");
        if (id == 1) body.Append(P("Графіки не надано."));
    }

    // All paragraph formatting is defined here: no implicit Word styles.
    private static Paragraph P(string text, int size = 12, bool bold = false, bool center = false,
        int before = 0, int after = 120, bool indent = false,
        bool keepNext = false, bool newPage = false, int? hanging = null)
    {
        var pp = new ParagraphProperties();
        if (keepNext) pp.Append(new KeepNext());
        if (keepNext) pp.Append(new KeepLines());
        if (newPage) pp.Append(new PageBreakBefore());
        pp.Append(new SpacingBetweenLines
        {
            Before = before.ToString(CultureInfo.InvariantCulture),
            After = after.ToString(CultureInfo.InvariantCulture),
            Line = "300",
            LineRule = LineSpacingRuleValues.Auto
        });
        if (indent) pp.Append(new Indentation { FirstLine = "600" });
        if (hanging.HasValue) pp.Append(new Indentation { Left = "720", Hanging = hanging.Value.ToString(CultureInfo.InvariantCulture) });
        if (center) pp.Append(new Justification { Val = JustificationValues.Center });

        var rp = new RunProperties(new RunFonts { Ascii = Font, HighAnsi = Font, EastAsia = Font, ComplexScript = Font });
        if (bold) rp.Append(new Bold());
        rp.Append(new FontSize { Val = (size * 2).ToString(CultureInfo.InvariantCulture) });
        return new Paragraph(pp, new Run(rp, new Text(text ?? "") { Space = SpaceProcessingModeValues.Preserve }));
    }

    private static Paragraph H(string text, int level = 1, bool newPage = false) =>
        P(text, level == 1 ? 14 : 12, bold: true, before: level == 1 ? 220 : 150,
          after: 110, keepNext: true, newPage: newPage);

    // A literal bullet avoids an additional numbering part; hanging indentation keeps wrapped lines aligned.
    private static Paragraph Bullet(string text) => P("•  " + text, after: 50, hanging: 360);

    private static Table MakeTable(int[] widths, bool borders = true)
    {
        var props = new TableProperties(
            new TableWidth { Width = ContentWidth.ToString(CultureInfo.InvariantCulture), Type = TableWidthUnitValues.Dxa });
        if (borders)
            props.Append(new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4 },
                new LeftBorder { Val = BorderValues.Single, Size = 4 },
                new BottomBorder { Val = BorderValues.Single, Size = 4 },
                new RightBorder { Val = BorderValues.Single, Size = 4 },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }));
        props.Append(new TableLayout { Type = TableLayoutValues.Fixed });
        var grid = new TableGrid();
        foreach (var width in widths)
            grid.Append(new GridColumn { Width = width.ToString(CultureInfo.InvariantCulture) });
        return new Table(props, grid);
    }

    private static void AddRow(Table table, int[] widths, string[] values, bool header = false)
    {
        var rowProps = new TableRowProperties(new CantSplit());
        if (header) rowProps.Append(new TableHeader());
        var row = new TableRow(rowProps);
        for (int i = 0; i < widths.Length; i++)
        {
            var cp = new TableCellProperties(
                new TableCellWidth { Width = widths[i].ToString(CultureInfo.InvariantCulture), Type = TableWidthUnitValues.Dxa });
            if (header) cp.Append(new Shading { Fill = HeaderFill, Val = ShadingPatternValues.Clear });
            cp.Append(new TableCellMargin(
                new TopMargin { Width = "70", Type = TableWidthUnitValues.Dxa },
                new LeftMargin { Width = "100", Type = TableWidthUnitValues.Dxa },
                new BottomMargin { Width = "70", Type = TableWidthUnitValues.Dxa },
                new RightMargin { Width = "100", Type = TableWidthUnitValues.Dxa }));
            cp.Append(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
            row.Append(new TableCell(cp, P(values[i], 10, bold: header, after: 0)));
        }
        table.Append(row);
    }

    private static void AddImage(MainDocumentPart main, Body body, byte[] png, uint id, string description)
    {
        //png зберігає розміри у своєму блоці IHDR; бібліотека для обробки зображень не потрібна.
        if (png.Length < 24 || !png.AsSpan(0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }) ||
            !png.AsSpan(12, 4).SequenceEqual(new byte[] { 73, 72, 68, 82 }))
            throw new ArgumentException("Графіки повинні бути у форматі PNG.");
        uint width = BinaryPrimitives.ReadUInt32BigEndian(png.AsSpan(16, 4));
        uint height = BinaryPrimitives.ReadUInt32BigEndian(png.AsSpan(20, 4));
        if (width == 0 || height == 0) throw new ArgumentException("Некоректний розмір PNG.");
        double scale = Math.Min((double)ImageMaxWidth / width, (double)ImageMaxHeight / height);
        long cx = (long)Math.Round(width * scale);
        long cy = (long)Math.Round(height * scale);

        var part = main.AddImagePart(ImagePartType.Png);
        using (var input = new MemoryStream(png, writable: false)) part.FeedData(input);
        string rel = main.GetIdOfPart(part);
        var drawing = new Drawing(new DW.Inline(
            new DW.Extent { Cx = cx, Cy = cy },
            new DW.DocProperties { Id = id, Name = $"Chart {id}", Description = description },
            new A.Graphic(new A.GraphicData(
                new PIC.Picture(
                    new PIC.NonVisualPictureProperties(
                        new PIC.NonVisualDrawingProperties { Id = id, Name = $"Chart {id}" },
                        new PIC.NonVisualPictureDrawingProperties()),
                    new PIC.BlipFill(new A.Blip { Embed = rel }, new A.Stretch(new A.FillRectangle())),
                    new PIC.ShapeProperties(
                        new A.Transform2D(new A.Offset { X = 0, Y = 0 }, new A.Extents { Cx = cx, Cy = cy }),
                        new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle })))
            { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }))
        { DistanceFromTop = 0, DistanceFromBottom = 0, DistanceFromLeft = 0, DistanceFromRight = 0 });

        body.Append(new Paragraph(
            new ParagraphProperties(
                new KeepNext(), new KeepLines(),
                new SpacingBetweenLines { Before = "120", After = "0" },
                new Justification { Val = JustificationValues.Center }),
            new Run(drawing)));
        body.Append(P($"Рисунок {id} — {description}", 10, center: true, after: 180));
    }

    private static string N(double number) => double.IsFinite(number) ? number.ToString("N2", Ua) : "—";
    private static string Or(string? value, string fallback) => string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
}
