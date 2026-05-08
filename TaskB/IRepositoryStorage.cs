namespace TaskB;

public interface IRepositoryStorage
{
    bool TryAdd(string itemName, RepositoryItem item);
    bool TryGet(string itemName, out RepositoryItem item);
    bool TryRemove(string itemName, out RepositoryItem removedItem);
}
