using FolderSync.Domain.Enums;

namespace FolderSync.Application.DTOs;

public class SyncConfigDto
{
    public string SourceDirectory { get; set; } = string.Empty;
    public string TargetDirectory { get; set; } = string.Empty;
    public SyncDirection Direction { get; set; } = SyncDirection.SourceToTarget;
}