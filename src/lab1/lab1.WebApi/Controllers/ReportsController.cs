using lab1.Application.DTOs;
using lab1.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace lab1.WebApi.Controllers;

[ApiController]
[Route("api/v2/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("xlsx")]
    public async Task<IActionResult> GetXlsxReport()
    {
        var stream = await _reportService.CreateXlsxReportAsync();

        return File(
            stream,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "air-quality-report.xlsx");
    }

    [HttpPost("docx")]
    public async Task<IActionResult> GetDocxReport([FromBody] ReportMetadataDto metadata)
    {
        var stream = await _reportService.CreateDocxReportAsync(metadata);

        return File(
            stream,
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "air-quality-report.docx");
    }
}
