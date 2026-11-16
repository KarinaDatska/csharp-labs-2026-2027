using lab1.Application.DTOs;
using lab1.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace lab1.WebApi.Controllers;

[ApiController]
[Route("api/v2/[controller]")]
public class CsvController : ControllerBase
{
    private readonly ICsvAirQualityService _csvDataService;

    public CsvController(ICsvAirQualityService csvDataService) => _csvDataService = csvDataService;

    [HttpPost("import")]
    public async Task<ActionResult<IReadOnlyList<AirQualityDto>>> ReadFromCsv(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var records = await _csvDataService.ReadFromCsvAsync(stream, file.FileName);
        return Ok(records);
    }

    [HttpGet("export")]
    public async Task<ActionResult> WriteToCsv()
    {
        var stream = await _csvDataService.WriteToCsvAsync();
        return File(stream, "text/csv", "air_quality.csv");
    }
}