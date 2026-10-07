using lab1.Application.DTOs;
using lab1.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace lab1.WebApi.Controllers;

[ApiController]
[Route("api/v2/[controller]")]
public class XmlController : ControllerBase
{
    private readonly IXmlAirQualityService _xmlDataService;

    public XmlController(IXmlAirQualityService xmlDataService) => _xmlDataService = xmlDataService;

    [HttpPost("import")]
    public async Task<ActionResult<IReadOnlyList<AirQualityDto>>> ReadFromXml(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var records = await _xmlDataService.ReadFromXmlAsync(stream);
        return Ok(records);
    }

    [HttpGet("export")]
    public async Task<ActionResult> WriteToXml()
    {
        var stream = await _xmlDataService.WriteToXmlAsync();
        return File(stream, "text/xml", "air_quality.xml");
    }
}