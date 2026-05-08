namespace TaskB;

public interface IContentValidator
{
    bool Validate(string content, ItemType type);
}
