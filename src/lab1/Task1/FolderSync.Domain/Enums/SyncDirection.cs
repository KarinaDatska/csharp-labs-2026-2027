namespace FolderSync.Domain.Enums
{
    public enum SyncDirection
    {
        SourceToTarget, // з джерела в призначення
        TargetToSource, // з призначення в джерело
        Bidirectional   // двостороння синхронізація
    }
}