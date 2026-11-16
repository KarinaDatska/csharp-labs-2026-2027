using lab1.Application.DTOs;
using lab1.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace lab1.WebApi.Controllers;

[ApiController]
[Route("api/v2/[controller]")]
public class ChartsController : ControllerBase
{
    private readonly IChartDataService _chartDataService;

    public ChartsController(IChartDataService chartDataService)
    {
        _chartDataService = chartDataService;
    }

    [HttpGet("top-polluted")]
    [Produces("image/png")]
    public IActionResult GetTopPollutedChart([FromQuery] AirQualityFilter filter)
    {
        var chart = _chartDataService.GetTopPollutedBarChart(filter);
        if (chart == null)
        {
            return NoContent();
        }

        return File(chart, "image/png");
    }

    [HttpGet("distribution")]
    [Produces("image/png")]
    public IActionResult GetPollutionDistributionChart([FromQuery] AirQualityFilter filter)
    {
        var chart = _chartDataService.GetPollutionDistributionPieChart(filter);
        if (chart == null)
        {
            return NoContent();
        }

        return File(chart, "image/png");
    }

    [HttpGet("monthly-trend")]
    [Produces("image/png")]
    public IActionResult GetMonthlyTrendChart([FromQuery] AirQualityFilter filter)
    {
        var chart = _chartDataService.GetMonthlyTrendLineChart(filter);
        if (chart == null)
        {
            return NoContent();
        }

        return File(chart, "image/png");
    }
}
