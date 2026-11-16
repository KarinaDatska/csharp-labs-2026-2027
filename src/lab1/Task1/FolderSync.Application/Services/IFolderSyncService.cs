using FolderSync.Application.DTOs;
using FolderSync.Domain.Entities;
using FolderSync.Domain.Enums;

namespace FolderSync.Application.Services;

public interface IFolderSyncService
{
    List<SyncItem> AnalyzeFolders(string sourceDir, string targetDir, SyncDirection direction);
    SyncResultDto ExecuteSynchronization(string sourceDir, string targetDir, SyncDirection direction);
}
