using lab1.Application.DTOs;
using lab1.Application.Services;
using lab1.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace lab1.WebApi.Controllers;

[ApiController]
[Route("api/data")]
public class DataController(RecordService service) : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromQuery] AirQualityFilter filter, string sortBy = "Rank",
        bool descending = false, int page = 1, int pageSize = 50) =>
        Ok(service.Query(filter, sortBy, descending, page, pageSize));

    [HttpPost]
    public IActionResult Add(AirQualityRecord record) => Ok(service.Save(record, null));

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, AirQualityRecord record) => Ok(service.Save(record, id));

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        service.Delete(id);
        return NoContent();
    }
}
