using System.Collections.Concurrent;

namespace TaskB;

public class InMemoryStorage : IRepositoryStorage
{
    private readonly ConcurrentDictionary<string, RepositoryItem> _items = new();

    public bool TryAdd(string itemName, RepositoryItem item) => _items.TryAdd(itemName, item);

    public bool TryGet(string itemName, out RepositoryItem item) => _items.TryGetValue(itemName, out item);

    public bool TryRemove(string itemName, out RepositoryItem removedItem) => _items.TryRemove(itemName, out removedItem);
}
