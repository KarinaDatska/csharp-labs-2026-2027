using lab1.Application.DTOs;
using lab1.Domain.Entities;
using lab1.Domain.Enums;

namespace lab1.Application.Services;

public interface IFolderSyncService
{
    List<SyncItem> AnalyzeFolders(string sourceDir, string targetDir, SyncDirection direction);
    SyncResultDto ExecuteSynchronization(string sourceDir, string targetDir, SyncDirection direction);
}
