using FolderSync.Application.Repositories;

namespace FolderSync.Infrastructure.Repositories;

public class TestFileGenerator : ITestFileGenerator
{
    public int Generate(
        string rootPath,
        int folderCount,
        int filesPerFolder)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            throw new ArgumentException("Root path cannot be empty.", nameof(rootPath));
        }

        if (folderCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(folderCount), "Folder count must be greater than zero.");
        }

        if (filesPerFolder <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(filesPerFolder), "Files per folder must be greater than zero.");
        }

        Directory.CreateDirectory(rootPath);
        var createdFiles = 0;

        for (var folderIndex = 1; folderIndex <= folderCount; folderIndex++)
        {
            var folderPath = Path.Combine(rootPath, $"Folder_{folderIndex}");
            Directory.CreateDirectory(folderPath);

            for (var fileIndex = 1; fileIndex <= filesPerFolder; fileIndex++)
            {
                var filePath = Path.Combine(folderPath, $"File_{fileIndex}.txt");

                if (File.Exists(filePath))
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                }

                File.WriteAllText(filePath, $"Test file {fileIndex} in folder {folderIndex}");
                SetRandomAttribute(filePath);
                createdFiles++;
            }
        }

        return createdFiles;
    }

    private static void SetRandomAttribute(string filePath)
    {
        var attributes = OperatingSystem.IsWindows()
            ? new[]
            {
                FileAttributes.Normal,
                FileAttributes.ReadOnly,
                FileAttributes.Hidden,
                FileAttributes.Archive
            }
            : new[]
            {
                FileAttributes.Normal,
                FileAttributes.ReadOnly
            };

        var attribute = attributes[Random.Shared.Next(attributes.Length)];
        File.SetAttributes(filePath, attribute);
    }
}
