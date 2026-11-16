using System.Security.Cryptography;
using FolderSync.Application.DTOs;
using FolderSync.Domain.Entities;
using FolderSync.Domain.Enums;

namespace FolderSync.Application.Services;

public class FolderSyncService : IFolderSyncService
{
    public List<SyncItem> AnalyzeFolders(string sourceDir, string targetDir, SyncDirection direction)
    {
        var source = Normalize(sourceDir);
        var target = Normalize(targetDir);
        if (source.Equals(target, StringComparison.OrdinalIgnoreCase) ||
            source.StartsWith(target + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
            target.StartsWith(source + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Оберіть різні папки, які не вкладені одна в одну.");
        if (!Enum.IsDefined(direction)) throw new ArgumentException("Невідомий напрямок.");
        if (direction == SyncDirection.TargetToSource) (source, target) = (target, source);
        if (!Directory.Exists(source)) throw new DirectoryNotFoundException("Папку джерела не знайдено.");
        var items = new List<SyncItem>();
        Compare(source, target, items, direction == SyncDirection.Bidirectional);
        return items;
    }

    private static string Normalize(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Вкажіть обидві папки.");
        return Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
    }

    private static Dictionary<string, string> Files(string path)
    {
        if (!Directory.Exists(path)) return new(StringComparer.OrdinalIgnoreCase);
        var options = new EnumerationOptions { RecurseSubdirectories = true,
            AttributesToSkip = FileAttributes.ReparsePoint, IgnoreInaccessible = false };
        return Directory.EnumerateFiles(path, "*", options)
            .ToDictionary(file => Path.GetRelativePath(path, file), StringComparer.OrdinalIgnoreCase);
    }

    private static void Compare(string source, string target, List<SyncItem> items, bool both)
    {
        var sourceFiles = Files(source);
        var targetFiles = Files(target);
        foreach (var (relative, file) in sourceFiles)
        {
            var destination = Path.Combine(target, relative);
            if (!targetFiles.TryGetValue(relative, out var other))
                Add(items, relative, file, destination, FileChangeType.New);
            else if (!EqualContent(file, other))
            {
                if (both && File.GetLastWriteTimeUtc(other) > File.GetLastWriteTimeUtc(file))
                    Add(items, relative, other, file, FileChangeType.Modified);
                else Add(items, relative, file, other, FileChangeType.Modified);
            }
        }
        foreach (var (relative, file) in targetFiles)
        {
            if (sourceFiles.ContainsKey(relative)) continue;
            if (both) Add(items, relative, file, Path.Combine(source, relative), FileChangeType.New);
            else Add(items, relative, Path.Combine(source, relative), file, FileChangeType.Deleted);
        }
    }

    private static bool EqualContent(string first, string second)
    {
        if (new FileInfo(first).Length != new FileInfo(second).Length) return false;
        using var a = File.OpenRead(first);
        using var b = File.OpenRead(second);
        return SHA256.HashData(a).SequenceEqual(SHA256.HashData(b));
    }

    private static void Add(List<SyncItem> items, string relative, string source, string target, FileChangeType type)
    {
        items.Add(new SyncItem { RelativePath = relative, SourceFullPath = source,
            TargetFullPath = target, ChangeType = type,
            Description = type == FileChangeType.Deleted ? "Видалити" : "Копіювати" });
    }

    public SyncResultDto ExecuteSynchronization(string sourceDir, string targetDir, SyncDirection direction)
    {
        var items = AnalyzeFolders(sourceDir, targetDir, direction);
        var count = 0;
        var errors = new List<string>();
        foreach (var item in items)
        {
            try
            {
                if (item.ChangeType == FileChangeType.Deleted) File.Delete(item.TargetFullPath!);
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(item.TargetFullPath!)!);
                    File.Copy(item.SourceFullPath!, item.TargetFullPath!, true);
                }
                count++;
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException)
            {
                errors.Add($"{item.RelativePath}: {error.Message}");
            }
        }
        return new SyncResultDto { TotalAnalyzed = items.Count, SyncedFiles = count, Items = items,
            Message = $"Виконано {count} з {items.Count}. " + string.Join("; ", errors) };
    }
}
