using lab1.Application.DTOs;
using lab1.Application.Repositories;
using lab1.Domain.Entities;
using lab1.Domain.Enums;
using lab1.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using DataFileOptions = lab1.Application.DTOs.FileOptions;

namespace lab1.WebApi.Controllers;

[ApiController]
[Route("api/files")]
public class FilesController(IAirQualityDataStore store, ICsvAirQualityProvider csv,
    IJsonAirQualityProvider json, IXmlAirQualityProvider xml, IXlsxAirQualityProvider xlsx,
    DataFileOptions options, RecentFileStore recent) : ControllerBase
{
    [HttpPost("preview")]
    public async Task<IActionResult> Preview(IFormFile file, [FromQuery] DataFileOptions settings)
    {
        Configure(settings);
        await using var stream = file.OpenReadStream();
        var records = await Read(stream, file.FileName);
        foreach (var record in records) lab1.Domain.Validation.RecordValidator.Validate(record);
        var missing = records.Sum(r => typeof(MonthlyMeasurements).GetProperties()
            .Count(p => p.GetValue(r.Measurements) == null));
        return Ok(new { rows = records.Count, columns = 15, missing,
            types = new { rank = "int", city = "string", avg = "double", months = "double?" },
            preview = records.Take(10) });
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import(IFormFile file, [FromQuery] DataFileOptions settings)
    {
        Configure(settings);
        await using var stream = file.OpenReadStream();
        return await ImportStream(stream, file.FileName);
    }

    private async Task<IActionResult> ImportStream(Stream stream, string name)
    {
        var records = (await Read(stream, name)).ToList();
        foreach (var record in records) lab1.Domain.Validation.RecordValidator.Validate(record);
        await recent.Remember(name, stream);
        store.SetData(records, Format(name));
        return Ok(new { name, count = records.Count });
    }

    [HttpGet("recent")]
    public IActionResult Recent() => Ok(recent.List());

    [HttpPost("recent/{id}")]
    public async Task<IActionResult> OpenRecent(string id, [FromQuery] DataFileOptions settings)
    {
        Configure(settings);
        var file = recent.Open(id);
        await using var stream = file.Stream;
        return await ImportStream(stream, file.Name);
    }

    [HttpGet("export/{format}")]
    public async Task<IActionResult> Export(string format, [FromQuery] DataFileOptions settings)
    {
        Configure(settings);
        var records = store.Records.ToList();
        foreach (var record in records) lab1.Domain.Validation.RecordValidator.Validate(record);
        var stream = format.ToLowerInvariant() switch
        {
            "csv" => await csv.WriteToCsvAsync(records),
            "json" => await json.WriteToJsonAsync(records),
            "xml" => await xml.WriteToXmlAsync(records),
            "xlsx" => await xlsx.WriteToXlsxAsync(records),
            _ => throw new ArgumentException("Оберіть CSV, JSON, XML або XLSX.")
        };
        return File(stream, "application/octet-stream", "air_quality." + format.ToLowerInvariant());
    }

    private Task<IReadOnlyList<AirQualityRecord>> Read(Stream stream, string name) => Format(name) switch
    {
        DataFormat.Csv => csv.ReadFromCsvAsync(stream),
        DataFormat.Json => json.ReadFromJsonAsync(stream),
        DataFormat.Xml => xml.ReadFromXmlAsync(stream),
        DataFormat.Xlsx => xlsx.ReadFromXlsxAsync(stream),
        _ => throw new ArgumentException("Оберіть CSV, JSON, XML або XLSX.")
    };

    private static DataFormat Format(string name) => Path.GetExtension(name).ToLowerInvariant() switch
    {
        ".csv" => DataFormat.Csv, ".json" => DataFormat.Json,
        ".xml" => DataFormat.Xml, ".xlsx" => DataFormat.Xlsx,
        _ => throw new ArgumentException("Оберіть CSV, JSON, XML або XLSX.")
    };

    private void Configure(DataFileOptions settings)
    {
        settings.Validate();
        options.EncodingName = settings.EncodingName;
        options.Delimiter = settings.Delimiter;
        options.HasHeader = settings.HasHeader;
        options.Culture = settings.Culture;
        options.Sheet = settings.Sheet;
    }
}
