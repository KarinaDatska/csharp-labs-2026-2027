using lab1.Application.DTOs;
using lab1.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace lab1.WebApi.Controllers;

[ApiController]
[Route("api/v2/[controller]")]
public class XlsxController : ControllerBase
{
    private readonly IXlsxAirQualityService _xlsxDataService;

    public XlsxController(IXlsxAirQualityService xlsxDataService) => _xlsxDataService = xlsxDataService;

    [HttpPost("import")]
    public async Task<ActionResult<IReadOnlyList<AirQualityDto>>> ReadFromXlsx(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var records = await _xlsxDataService.ReadFromXlsxAsync(stream);
        return Ok(records);
    }

    [HttpGet("export")]
    public async Task<ActionResult> WriteToXlsx()
    {
        var stream = await _xlsxDataService.WriteToXlsxAsync();
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "air_quality.xlsx");
    }
}