namespace lab1.Infrastructure.Logging;

public class EventLog
{
    private readonly object gate = new();
    private readonly string path = Path.Combine(AppContext.BaseDirectory, "Outputs", "events.log");

    public void Write(string message)
    {
        lock (gate)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.AppendAllText(path, $"{DateTime.Now:O} {message.Replace('\r', ' ').Replace('\n', ' ')}{Environment.NewLine}");
        }
    }
}
