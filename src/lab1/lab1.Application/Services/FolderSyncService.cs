using lab1.Application.DTOs;
using lab1.Domain.Entities;
using lab1.Domain.Enums;

namespace lab1.Application.Services;

public class FolderSyncService : IFolderSyncService
{
    public List<SyncItem> AnalyzeFolders(string sourceDir, string targetDir, SyncDirection direction)
    {
        var syncItems = new List<SyncItem>();

        if (string.IsNullOrWhiteSpace(sourceDir) || string.IsNullOrWhiteSpace(targetDir))
        {
            return syncItems;
        }

        if (!Directory.Exists(sourceDir))
        {
            Directory.CreateDirectory(sourceDir);
        }
        if (!Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
        }

        string[] sourceFiles = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories);
        string[] targetFiles = Directory.GetFiles(targetDir, "*.*", SearchOption.AllDirectories);

        for (int i = 0; i < sourceFiles.Length; i++)
        {
            string srcFile = sourceFiles[i];
            string relativePath = Path.GetRelativePath(sourceDir, srcFile);
            string tgtFile = Path.Combine(targetDir, relativePath);

            var syncItem = new SyncItem
            {
                RelativePath = relativePath,
                SourceFullPath = srcFile,
                TargetFullPath = tgtFile
            };

            if (!File.Exists(tgtFile))
            {
                syncItem.ChangeType = FileChangeType.New;
                syncItem.Description = "Файл відсутній у цільовій папці (буде створено)";
                syncItems.Add(syncItem);
            }
            else
            {
                DateTime srcTime = File.GetLastWriteTimeUtc(srcFile);
                DateTime tgtTime = File.GetLastWriteTimeUtc(tgtFile);

                if (srcTime > tgtTime)
                {
                    syncItem.ChangeType = FileChangeType.Modified;
                    syncItem.Description = "Файл змінено в джерелі (буде оновлено)";
                    syncItems.Add(syncItem);
                }
            }
        }

        for (int j = 0; j < targetFiles.Length; j++)
        {
            string tgtFile = targetFiles[j];
            string relativePath = Path.GetRelativePath(targetDir, tgtFile);
            string srcFile = Path.Combine(sourceDir, relativePath);

            if (!File.Exists(srcFile))
            {
                var syncItem = new SyncItem
                {
                    RelativePath = relativePath,
                    SourceFullPath = srcFile,
                    TargetFullPath = tgtFile,
                    ChangeType = FileChangeType.Deleted,
                    Description = "Файл відсутній у джерелі (буде видалено в цільовій)"
                };
                syncItems.Add(syncItem);
            }
        }

        return syncItems;
    }

    public SyncResultDto ExecuteSynchronization(string sourceDir, string targetDir, SyncDirection direction)
    {
        var items = AnalyzeFolders(sourceDir, targetDir, direction);
        var syncedCount = 0;

        foreach (var item in items)
        {
            try
            {
                if (item.ChangeType == FileChangeType.New || item.ChangeType == FileChangeType.Modified)
                {
                    if (item.SourceFullPath != null && item.TargetFullPath != null)
                    {
                        var dir = Path.GetDirectoryName(item.TargetFullPath);
                        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        File.Copy(item.SourceFullPath, item.TargetFullPath, true);
                        syncedCount++;
                    }
                }
                else if (item.ChangeType == FileChangeType.Deleted)
                {
                    if (item.TargetFullPath != null && File.Exists(item.TargetFullPath))
                    {
                        File.Delete(item.TargetFullPath);
                        syncedCount++;
                    }
                }
            }
            catch
            {
                // Помилки доступу або блоковані файли пропускаються
            }
        }

        return new SyncResultDto
        {
            TotalAnalyzed = items.Count,
            SyncedFiles = syncedCount,
            Items = items,
            Message = $"Успішно синхронізовано {syncedCount} з {items.Count} виявлених змін."
        };
    }
}