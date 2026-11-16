using lab1.Application.DTOs;
using lab1.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace lab1.WebApi.Controllers;

[ApiController]
[Route("api/v2/[controller]")]
public class RecordsController : ControllerBase
{
    private readonly IDataAnalyticsService _dataAnalyticsService;

    public RecordsController(IDataAnalyticsService dataAnalyticsService)
    {
        _dataAnalyticsService = dataAnalyticsService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var records = _dataAnalyticsService.GetAllRecords()
            .Select(CsvAirQualityService.MapToDto);
        return Ok(records);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetRecordById(int id)
    {
        var record = _dataAnalyticsService.GetRecordById(id);
        if (record == null)
        {
            return NotFound($"Запис з ID {id} не знайдено.");
        }

        return Ok(CsvAirQualityService.MapToDto(record));
    }

    [HttpGet("search")]
    public IActionResult SearchByCity([FromQuery] string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return BadRequest("Параметр city обов'язковий.");
        }

        var record = _dataAnalyticsService.FindByCity(city);
        if (record == null)
        {
            return NotFound($"Місто '{city}' не знайдено.");
        }

        return Ok(CsvAirQualityService.MapToDto(record));
    }

    [HttpGet("filters")]
    public IActionResult FilterRecords([FromQuery] AirQualityFilter filter)
    {
        var records = _dataAnalyticsService.FilterRecords(filter)
            .Select(CsvAirQualityService.MapToDto);
        return Ok(records);
    }

    [HttpGet("sort")]
    public IActionResult SortRecords([FromQuery] AirQualitySort sort)
    {
        var records = _dataAnalyticsService.SortRecords(sort)
            .Select(CsvAirQualityService.MapToDto);
        return Ok(records);
    }

    [HttpGet("analytics/sum")]
    public IActionResult GetSum()
    {
        var sum = _dataAnalyticsService.GetSum();
        return Ok(Math.Round(sum, 2));
    }

    [HttpGet("analytics/median")]
    public IActionResult GetMedian()
    {
        var median = _dataAnalyticsService.GetMedian();
        return Ok(Math.Round(median, 2));
    }

    [HttpGet("analytics/min")]
    public IActionResult GetMin()
    {
        var record = _dataAnalyticsService.GetMin();
        if (record == null)
        {
            return NotFound();
        }

        return Ok(CsvAirQualityService.MapToDto(record));
    }

    [HttpGet("analytics/max")]
    public IActionResult GetMax()
    {
        var record = _dataAnalyticsService.GetMax();
        if (record == null)
        {
            return NotFound();
        }

        return Ok(CsvAirQualityService.MapToDto(record));
    }

    [HttpGet("analytics/count")]
    public IActionResult GetTotalCount()
    {
        var count = _dataAnalyticsService.GetTotalCount();
        return Ok(count);
    }
}