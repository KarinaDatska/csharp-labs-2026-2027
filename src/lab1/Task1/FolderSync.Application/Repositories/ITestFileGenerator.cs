namespace FolderSync.Application.Repositories;

public interface ITestFileGenerator
{
    int Generate(
        string rootPath,
        int folderCount,
        int filesPerFolder);
}
