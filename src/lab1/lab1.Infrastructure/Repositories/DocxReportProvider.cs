using System.Globalization;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using lab1.Application.DTOs;
using lab1.Application.Repositories;

using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace lab1.Infrastructure.Repositories;

public class DocxReportProvider : IDocxReportProvider
{
    private const string AccentColor = "1F4E78";
    private const string AccentLightColor = "D9EAF7";
    private const string AlternateRowColor = "F4F7FA";
    private const string BorderColor = "A6A6A6";
    private const string WhiteColor = "FFFFFF";

    private static readonly CultureInfo ReportCulture = CultureInfo.GetCultureInfo("uk-UA");

    public async Task<Stream> CreateReportAsync(
        ReportDataDto reportData,
        ReportMetadataDto metadata,
        byte[]? topPollutedChart,
        byte[]? distributionChart,
        byte[]? monthlyTrendChart)
    {
        await SaveChartsAsync(topPollutedChart, distributionChart, monthlyTrendChart);

        var outputStream = new MemoryStream();

        using (var document = WordprocessingDocument.Create(outputStream, WordprocessingDocumentType.Document, true))
        {
            var mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = new Body();
            mainPart.Document.Append(body);

            AddTitlePage(body, metadata);
            AddPageBreak(body);

            AddDatasetSection(body, metadata);
            AddMetricsSection(body, reportData);
            AddPageBreak(body);

            AddSummaryTablesSection(body, reportData);
            AddPageBreak(body);

            AddChartsSection(mainPart, body, topPollutedChart, distributionChart, monthlyTrendChart);

            AddSectionProperties(body);

            mainPart.Document.Save();
        }

        outputStream.Position = 0;
        return outputStream;
    }

    private static void AddTitlePage(Body body, ReportMetadataDto metadata)
    {
        body.Append(CreateCenteredParagraph("МІНІСТЕРСТВО ОСВІТИ І НАУКИ УКРАЇНИ", 20, false, before: 400, after: 100));
        body.Append(CreateCenteredParagraph("НАЦІОНАЛЬНИЙ УНІВЕРСИТЕТ «ОСТРОЗЬКА АКАДЕМІЯ»", 22, true, after: 600));

        body.Append(CreateCenteredParagraph("ЗВІТ", 36, true, before: 1000, after: 100));
        body.Append(CreateCenteredParagraph("до лабораторної роботи №1", 26, false, after: 100));
        body.Append(CreateCenteredParagraph("Аналіз та візуалізація даних якості повітря", 30, true, after: 80));
        body.Append(CreateCenteredParagraph("Дисципліна: Програмування мовою C# (.NET)", 22, false, after: 1200));

        var metaTable = CreateTable();
        AddMetaRow(metaTable, "Студент:", string.IsNullOrWhiteSpace(metadata.StudentName) ? "Студент" : metadata.StudentName);
        AddMetaRow(metaTable, "Група:", string.IsNullOrWhiteSpace(metadata.Group) ? "КН" : metadata.Group);
        AddMetaRow(metaTable, "Варіант:", string.IsNullOrWhiteSpace(metadata.Variant) ? "1" : metadata.Variant);
        body.Append(metaTable);

        body.Append(CreateCenteredParagraph("Острог — 2026", 20, false, before: 2000, after: 200));
    }

    private static void AddDatasetSection(Body body, ReportMetadataDto metadata)
    {
        body.Append(CreateHeading1("1. Опис предметної області та набору даних"));

        body.Append(CreateParagraph(
            "Предметна область цієї лабораторної роботи присвячена аналізу показників якості повітря (концентрації часток PM2.5) " +
            "у містах світу. Набір даних включає інформацію про назву та країну міста, його рейтинг, середньорічний рівень забруднення, " +
            "а також детальні помісячні виміри від січня до грудня.",
            indent: true));

        body.Append(CreateParagraph(
            $"Характеристика набору даних: {metadata.DatasetDescription}",
            indent: true));

        body.Append(CreateParagraph("Основні атрибути набору даних:", bold: true));

        body.Append(CreateBulletPoint("Rank — порядковий номер міста за рівнем забруднення;"));
        body.Append(CreateBulletPoint("CityAndCountry — назва міста та країна спостереження;"));
        body.Append(CreateBulletPoint("AveragePollution — середньорічна концентрація дрібнодисперсних часток (PM2.5, мкг/м³);"));
        body.Append(CreateBulletPoint("MonthlyMeasurements — концентрація часток по місяцях (January – December)."));
    }

    private static void AddMetricsSection(Body body, ReportDataDto reportData)
    {
        body.Append(CreateHeading1("2. Загальні статистичні показники"));

        body.Append(CreateParagraph(
            "На основі завантаженого датасету було проведено аналітичну обробку та обчислення ключових описових статистик.",
            indent: true));

        var table = CreateStyledTable();
        AddTableHeader(table, ["Метрика", "Значення"]);

        AddDataRow(table, ["Загальна кількість спостережень (міст)", reportData.TotalRecords.ToString(ReportCulture)]);
        AddDataRow(table, ["Середній показник PM2.5 (середнє арифметичне)", FormatNumber(reportData.AveragePollutionMean)]);
        AddDataRow(table, ["Медіанний рівень PM2.5", FormatNumber(reportData.MedianPollution)]);
        AddDataRow(table, ["Мінімальний рівень PM2.5", FormatNumber(reportData.MinPollution)]);
        AddDataRow(table, ["Максимальний рівень PM2.5", FormatNumber(reportData.MaxPollution)]);

        body.Append(table);
    }

    private static void AddSummaryTablesSection(Body body, ReportDataDto reportData)
    {
        body.Append(CreateHeading1("3. Зведені аналітичні таблиці"));

        body.Append(CreateHeading2("3.1. Розподіл міст за категоріями безпеки повітря"));
        var categoryTable = CreateStyledTable();
        AddTableHeader(categoryTable, ["Категорія якості", "Кількість міст", "Частка (%)", "Середнє PM2.5"]);

        foreach (var row in reportData.ByCategory)
        {
            AddDataRow(categoryTable, [
                row.Category,
                row.RecordCount.ToString(ReportCulture),
                FormatNumber(row.Percentage) + "%",
                FormatNumber(row.AveragePollution)
            ]);
        }
        body.Append(categoryTable);

        body.Append(CreateHeading2("3.2. Сезонні коливання за місяцями (середні значення)"));
        var monthTable = CreateStyledTable();
        AddTableHeader(monthTable, ["Місяць", "Середній рівень PM2.5"]);

        foreach (var m in reportData.MonthlyAverages)
        {
            AddDataRow(monthTable, [
                m.Month,
                FormatNumber(m.AveragePollution)
            ]);
        }
        body.Append(monthTable);
    }

    private static void AddChartsSection(
        MainDocumentPart mainPart,
        Body body,
        byte[]? topPollutedChart,
        byte[]? distributionChart,
        byte[]? monthlyTrendChart)
    {
        body.Append(CreateHeading1("4. Графічна візуалізація"));

        uint imageId = 1;

        if (topPollutedChart is not null)
        {
            AddFigure(mainPart, body, topPollutedChart, imageId++, "Рисунок 1 — Топ-10 найбільш забруднених міст світу");
        }

        if (distributionChart is not null)
        {
            AddFigure(mainPart, body, distributionChart, imageId++, "Рисунок 2 — Розподіл міст за категоріями якості повітря");
        }

        if (monthlyTrendChart is not null)
        {
            AddFigure(mainPart, body, monthlyTrendChart, imageId, "Рисунок 3 — Сезонна динаміка забруднення повітря по місяцях");
        }
    }

    private static void AddFigure(MainDocumentPart mainPart, Body body, byte[] imageBytes, uint imageId, string caption)
    {
        var imagePart = mainPart.AddImagePart(ImagePartType.Png);
        using (var stream = new MemoryStream(imageBytes))
        {
            imagePart.FeedData(stream);
        }

        var relationshipId = mainPart.GetIdOfPart(imagePart);
        const long imageWidth = 5_394_960L;
        const long imageHeight = 3_236_976L;

        var drawing = new Drawing(
            new DW.Inline(
                new DW.Extent { Cx = imageWidth, Cy = imageHeight },
                new DW.EffectExtent { LeftEdge = 0, TopEdge = 0, RightEdge = 0, BottomEdge = 0 },
                new DW.DocProperties { Id = imageId, Name = caption },
                new DW.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks { NoChangeAspect = true }),
                new A.Graphic(
                    new A.GraphicData(
                        new PIC.Picture(
                            new PIC.NonVisualPictureProperties(
                                new PIC.NonVisualDrawingProperties { Id = imageId, Name = caption },
                                new PIC.NonVisualPictureDrawingProperties()),
                            new PIC.BlipFill(
                                new A.Blip { Embed = relationshipId, CompressionState = A.BlipCompressionValues.Print },
                                new A.Stretch(new A.FillRectangle())),
                            new PIC.ShapeProperties(
                                new A.Transform2D(new A.Offset { X = 0, Y = 0 }, new A.Extents { Cx = imageWidth, Cy = imageHeight }),
                                new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle }))
                        ) { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }))
            {
                DistanceFromTop = 0,
                DistanceFromBottom = 0,
                DistanceFromLeft = 0,
                DistanceFromRight = 0
            });

        var p = new Paragraph(new Run(drawing));
        p.ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Center });
        body.Append(p);

        body.Append(CreateCenteredParagraph(caption, 20, false, before: 60, after: 200));
    }

    private static async Task SaveChartsAsync(byte[]? chart1, byte[]? chart2, byte[]? chart3)
    {
        var directory = Path.Combine(Directory.GetCurrentDirectory(), "Reports", "Charts");
        Directory.CreateDirectory(directory);

        if (chart1 is not null) await File.WriteAllBytesAsync(Path.Combine(directory, "top-polluted.png"), chart1);
        if (chart2 is not null) await File.WriteAllBytesAsync(Path.Combine(directory, "distribution.png"), chart2);
        if (chart3 is not null) await File.WriteAllBytesAsync(Path.Combine(directory, "monthly-trend.png"), chart3);
    }

    private static Paragraph CreateHeading1(string text)
    {
        var p = CreateParagraph(text, 28, true, before: 240, after: 120);
        p.ParagraphProperties!.ParagraphStyleId = new ParagraphStyleId { Val = "Heading1" };
        return p;
    }

    private static Paragraph CreateHeading2(string text)
    {
        var p = CreateParagraph(text, 24, true, before: 180, after: 80);
        p.ParagraphProperties!.ParagraphStyleId = new ParagraphStyleId { Val = "Heading2" };
        return p;
    }

    private static Paragraph CreateParagraph(string text, int fontSize = 24, bool bold = false, int before = 0, int after = 100, bool indent = false)
    {
        var p = new Paragraph();
        var props = new ParagraphProperties
        {
            SpacingBetweenLines = new SpacingBetweenLines { Before = before.ToString(), After = after.ToString(), Line = "276", LineRule = LineSpacingRuleValues.Auto }
        };

        if (indent) props.Indentation = new Indentation { FirstLine = "720" };
        p.Append(props);

        var run = new Run();
        var runProps = new RunProperties(new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }, new FontSize { Val = fontSize.ToString() });
        if (bold) runProps.Bold = new Bold();
        run.Append(runProps);
        run.Append(new Text(text));
        p.Append(run);

        return p;
    }

    private static Paragraph CreateCenteredParagraph(string text, int fontSize = 24, bool bold = false, int before = 0, int after = 100)
    {
        var p = CreateParagraph(text, fontSize, bold, before, after);
        p.ParagraphProperties!.Justification = new Justification { Val = JustificationValues.Center };
        return p;
    }

    private static Paragraph CreateBulletPoint(string text)
    {
        var p = new Paragraph();
        p.ParagraphProperties = new ParagraphProperties
        {
            Indentation = new Indentation { Left = "720", Hanging = "360" },
            SpacingBetweenLines = new SpacingBetweenLines { After = "60" }
        };

        var runBullet = new Run(new RunProperties(new FontSize { Val = "22" }), new Text("•  "));
        var runText = new Run(new RunProperties(new FontSize { Val = "22" }), new Text(text));
        p.Append(runBullet, runText);
        return p;
    }

    private static void AddPageBreak(Body body)
    {
        body.Append(new Paragraph(new Run(new Break { Type = BreakValues.Page })));
    }

    private static Table CreateTable() => new(new TableProperties(new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }));

    private static Table CreateStyledTable()
    {
        var table = new Table();
        table.Append(new TableProperties(
            new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct },
            new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4, Color = BorderColor },
                new BottomBorder { Val = BorderValues.Single, Size = 4, Color = BorderColor },
                new LeftBorder { Val = BorderValues.None },
                new RightBorder { Val = BorderValues.None },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4, Color = BorderColor },
                new InsideVerticalBorder { Val = BorderValues.None }
            )
        ));
        return table;
    }

    private static void AddTableHeader(Table table, string[] headers)
    {
        var row = new TableRow();
        foreach (var header in headers)
        {
            var cell = new TableCell(
                new TableCellProperties(
                    new Shading { Fill = AccentColor },
                    new TableCellMargin(new TopMargin { Width = "120" }, new BottomMargin { Width = "120" })
                ),
                new Paragraph(new Run(
                    new RunProperties(new Bold(), new Color { Val = WhiteColor }, new FontSize { Val = "20" }),
                    new Text(header)
                ))
            );
            row.Append(cell);
        }
        table.Append(row);
    }

    private static void AddDataRow(Table table, string[] values)
    {
        var row = new TableRow();
        foreach (var val in values)
        {
            var cell = new TableCell(
                new TableCellProperties(
                    new TableCellMargin(new TopMargin { Width = "80" }, new BottomMargin { Width = "80" })
                ),
                new Paragraph(new Run(
                    new RunProperties(new FontSize { Val = "20" }),
                    new Text(val)
                ))
            );
            row.Append(cell);
        }
        table.Append(row);
    }

    private static void AddMetaRow(Table table, string label, string value)
    {
        var row = new TableRow();
        row.Append(new TableCell(new Paragraph(new Run(new RunProperties(new Bold(), new FontSize { Val = "22" }), new Text(label)))));
        row.Append(new TableCell(new Paragraph(new Run(new RunProperties(new FontSize { Val = "22" }), new Text(value)))));
        table.Append(row);
    }

    private static void AddSectionProperties(Body body)
    {
        var sectionProps = new SectionProperties(
            new PageMargin { Top = 1440, Bottom = 1440, Left = 1440, Right = 1440 }
        );
        body.Append(sectionProps);
    }

    private static string FormatNumber(double value) => value.ToString("N2", ReportCulture);
}
