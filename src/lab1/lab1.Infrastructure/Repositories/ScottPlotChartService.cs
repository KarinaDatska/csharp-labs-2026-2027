using lab1.Application.Repositories;
using lab1.Domain.Entities;
using ScottPlot;

namespace lab1.Infrastructure.Repositories;

public class ScottPlotChartService : IScottPlotChartService
{
    private const int Width = 1000;
    private const int Height = 600;

    public byte[] CreateTopPollutedBarChart(IEnumerable<AirQualityRecord> records)
    {
        var top = records
            .OrderByDescending(r => r.AveragePollution)
            .Take(10)
            .ToList();

        var plot = new Plot();

        var bars = top
            .Select((r, i) => new Bar
            {
                Position = i,
                Value = r.AveragePollution,
                Label = r.AveragePollution.ToString("0.0")
            })
            .ToArray();

        plot.Add.Bars(bars);

        double[] positions = Enumerable.Range(0, top.Count).Select(i => (double)i).ToArray();
        string[] labels = top.Select(r => r.CityAndCountry.Length > 20 ? r.CityAndCountry[..20] + "..." : r.CityAndCountry).ToArray();

        plot.Axes.Bottom.SetTicks(positions, labels);
        plot.Axes.Bottom.TickLabelStyle.Rotation = 30;

        plot.Title("Топ-10 міст за середнім рівнем забруднення (PM2.5)");
        plot.XLabel("Місто");
        plot.YLabel("Рівень забруднення (PM2.5)");
        plot.Axes.Margins(bottom: 0, top: 0.15);

        return plot.GetImageBytes(Width, Height, ImageFormat.Png);
    }

    public byte[] CreatePollutionDistributionPieChart(IEnumerable<AirQualityRecord> records)
    {
        var list = records.ToList();
        var total = list.Count;

        if (total == 0)
        {
            var emptyPlot = new Plot();
            emptyPlot.Title("Немає даних");
            return emptyPlot.GetImageBytes(Width, Height, ImageFormat.Png);
        }

        var good = list.Count(r => r.AveragePollution <= 50);
        var moderate = list.Count(r => r.AveragePollution > 50 && r.AveragePollution <= 100);
        var unhealthy = list.Count(r => r.AveragePollution > 100 && r.AveragePollution <= 150);
        var veryUnhealthy = list.Count(r => r.AveragePollution > 150);

        List<PieSlice> slices =
        [
            new PieSlice { Value = good, FillColor = Colors.Green, Label = $"{good * 100.0 / total:0.0}%", LegendText = $"Добра (<=50): {good}" },
            new PieSlice { Value = moderate, FillColor = Colors.Gold, Label = $"{moderate * 100.0 / total:0.0}%", LegendText = $"Помірна (51-100): {moderate}" },
            new PieSlice { Value = unhealthy, FillColor = Colors.Orange, Label = $"{unhealthy * 100.0 / total:0.0}%", LegendText = $"Шкідлива (101-150): {unhealthy}" },
            new PieSlice { Value = veryUnhealthy, FillColor = Colors.Red, Label = $"{veryUnhealthy * 100.0 / total:0.0}%", LegendText = $"Дуже шкідлива (>150): {veryUnhealthy}" }
        ];

        var plot = new Plot();
        var activeSlices = slices.Where(s => s.Value > 0).ToList();
        if (activeSlices.Count > 0)
        {
            var pie = plot.Add.Pie(activeSlices);
            pie.SliceLabelDistance = 0.6;
        }

        plot.Title("Розподіл міст за рівнем якості повітря");
        plot.ShowLegend(Alignment.UpperRight);
        plot.Axes.Frameless();
        plot.HideGrid();

        return plot.GetImageBytes(Width, Height, ImageFormat.Png);
    }

    public byte[] CreateMonthlyTrendLineChart(IEnumerable<AirQualityRecord> records)
    {
        var list = records.ToList();
        var monthlyAverages = new[]
        {
            AverageOrZero(list.Select(r => r.Measurements?.January)),
            AverageOrZero(list.Select(r => r.Measurements?.February)),
            AverageOrZero(list.Select(r => r.Measurements?.March)),
            AverageOrZero(list.Select(r => r.Measurements?.April)),
            AverageOrZero(list.Select(r => r.Measurements?.May)),
            AverageOrZero(list.Select(r => r.Measurements?.June)),
            AverageOrZero(list.Select(r => r.Measurements?.July)),
            AverageOrZero(list.Select(r => r.Measurements?.August)),
            AverageOrZero(list.Select(r => r.Measurements?.September)),
            AverageOrZero(list.Select(r => r.Measurements?.October)),
            AverageOrZero(list.Select(r => r.Measurements?.November)),
            AverageOrZero(list.Select(r => r.Measurements?.December))
        };

        var plot = new Plot();
        double[] xs = Enumerable.Range(0, 12).Select(i => (double)i).ToArray();
        var scatter = plot.Add.Scatter(xs, monthlyAverages);
        scatter.LineWidth = 3;
        scatter.MarkerSize = 7;

        string[] months = ["Січ", "Лют", "Бер", "Кві", "Тра", "Чер", "Лип", "Сер", "Вер", "Жов", "Лис", "Гру"];
        plot.Axes.Bottom.SetTicks(xs, months);

        plot.Title("Сезонна динаміка середнього забруднення (по місяцях)");
        plot.XLabel("Місяць");
        plot.YLabel("Середнє PM2.5");

        return plot.GetImageBytes(Width, Height, ImageFormat.Png);
    }

    private static double AverageOrZero(IEnumerable<double?> values)
    {
        var valid = values.Where(v => v.HasValue).Select(v => v!.Value).ToList();
        return valid.Count > 0 ? Math.Round(valid.Average(), 2) : 0;
    }
}
