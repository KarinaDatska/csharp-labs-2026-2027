using lab1.Application.DTOs;
using lab1.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace lab1.WebApi.Controllers;

[ApiController]
[Route("api/v2/[controller]")]
public class JsonController : ControllerBase
{
    private readonly IJsonAirQualityService _jsonDataService;

    public JsonController(IJsonAirQualityService jsonDataService) => _jsonDataService = jsonDataService;

    [HttpPost("import")]
    public async Task<ActionResult<IReadOnlyList<AirQualityDto>>> ReadFromJson(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var records = await _jsonDataService.ReadFromJsonAsync(stream);
        return Ok(records);
    }

    [HttpGet("export")]
    public async Task<ActionResult> WriteToJson()
    {
        var stream = await _jsonDataService.WriteToJsonAsync();
        return File(stream, "application/json", "air_quality.json");
    }
}