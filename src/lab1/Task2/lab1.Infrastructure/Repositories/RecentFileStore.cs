using System.Text.Json;

namespace lab1.Infrastructure.Repositories;

public class RecentFileStore
{
    private readonly string directory = Path.Combine(AppContext.BaseDirectory, "Outputs", "Recent");
    private readonly object gate = new();
    public record Entry(string Id, string Name, DateTime Opened);

    public List<Entry> List()
    {
        lock (gate)
        {
            var path = Path.Combine(directory, "index.json");
            return File.Exists(path) ? JsonSerializer.Deserialize<List<Entry>>(File.ReadAllText(path)) ?? [] : [];
        }
    }

    public async Task Remember(string name, Stream stream)
    {
        Directory.CreateDirectory(directory);
        var entry = new Entry(Guid.NewGuid().ToString("N"), Path.GetFileName(name), DateTime.Now);
        await using (var output = File.Create(Path.Combine(directory, entry.Id)))
        {
            stream.Position = 0;
            await stream.CopyToAsync(output);
        }
        lock (gate)
        {
            var entries = List();
            entries.Insert(0, entry);
            foreach (var old in entries.Skip(10)) File.Delete(Path.Combine(directory, old.Id));
            File.WriteAllText(Path.Combine(directory, "index.json"), JsonSerializer.Serialize(entries.Take(10)));
        }
    }

    public (Stream Stream, string Name) Open(string id)
    {
        var entry = List().FirstOrDefault(e => e.Id == id) ?? throw new KeyNotFoundException("Файл не знайдено.");
        return (File.OpenRead(Path.Combine(directory, entry.Id)), entry.Name);
    }
}
