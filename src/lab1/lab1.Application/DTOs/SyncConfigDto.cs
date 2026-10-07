using lab1.Domain.Enums;

namespace lab1.Application.DTOs;

public class SyncConfigDto
{
    public string SourceDirectory { get; set; } = string.Empty;
    public string TargetDirectory { get; set; } = string.Empty;
    public SyncDirection Direction { get; set; } = SyncDirection.SourceToTarget;
}