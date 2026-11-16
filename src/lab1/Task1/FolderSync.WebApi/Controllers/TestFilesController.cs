using FolderSync.Application.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FolderSync.WebApi.Controllers;

[ApiController]
[Route("api/v1/test-files")]
public class TestFilesController : ControllerBase
{
    private readonly ITestFileGenerator _testFileGenerator;

    public TestFilesController(ITestFileGenerator testFileGenerator)
    {
        _testFileGenerator = testFileGenerator;
    }

    [HttpPost("generate")]
    public IActionResult Generate(
        int folderCount = 10,
        int filesPerFolder = 10)
    {
        var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedFiles");

        var createdFiles = _testFileGenerator.Generate(
            rootPath,
            folderCount,
            filesPerFolder);

        return Ok(new
        {
            rootPath,
            folderCount,
            filesPerFolder,
            createdFiles
        });
    }
}
