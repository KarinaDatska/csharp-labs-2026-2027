using FolderSync.Domain.Entities;

namespace FolderSync.Application.DTOs;

public class SyncResultDto
{
    public int TotalAnalyzed { get; set; }
    public int SyncedFiles { get; set; }
    public IReadOnlyList<SyncItem> Items { get; set; } = [];
    public string Message { get; set; } = string.Empty;
}
