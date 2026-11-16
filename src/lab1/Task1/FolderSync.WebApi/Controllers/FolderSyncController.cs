using FolderSync.Application.DTOs;
using FolderSync.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FolderSync.WebApi.Controllers;

[ApiController]
[Route("api/v1/folder-sync")]
public class FolderSyncController : ControllerBase
{
    private readonly IFolderSyncService _folderSyncService;

    public FolderSyncController(IFolderSyncService folderSyncService)
    {
        _folderSyncService = folderSyncService;
    }

    [HttpPost("analyze")]
    public IActionResult Analyze([FromBody] SyncConfigDto config)
    {
        var changes = _folderSyncService.AnalyzeFolders(
            config.SourceDirectory,
            config.TargetDirectory,
            config.Direction);

        return Ok(changes);
    }

    [HttpPost("execute")]
    public IActionResult Execute([FromBody] SyncConfigDto config)
    {
        var result = _folderSyncService.ExecuteSynchronization(
            config.SourceDirectory,
            config.TargetDirectory,
            config.Direction);

        return Ok(result);
    }
}
