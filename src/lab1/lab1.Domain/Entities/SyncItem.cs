using lab1.Domain.Enums;

namespace lab1.Domain.Entities;

public class SyncItem
{
    public string RelativePath { get; set; } = string.Empty;
    public string? SourceFullPath { get; set; }
    public string? TargetFullPath { get; set; }
    public FileChangeType ChangeType { get; set; }
    public string Description { get; set; } = string.Empty;
}