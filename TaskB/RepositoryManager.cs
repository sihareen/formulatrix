using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskB
{
    public class RepositoryManager : IDisposable
    {
        private readonly IRepositoryStorage _storage;
        private readonly IContentValidator _validator;
        private bool _isInitialized;
        private bool _disposed;

        public RepositoryManager() : this(new InMemoryStorage(), new ContentValidator()) { }

        public RepositoryManager(IRepositoryStorage storage, IContentValidator validator)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public void Initialize()
        {
            if (_isInitialized) throw new InvalidOperationException("Repository already initialized.");
            _isInitialized = true;
        }

        public void Register(string itemName, string itemContent, int itemType)
        {
            if (!_isInitialized) throw new InvalidOperationException("Initialize() must be called first.");
            if (string.IsNullOrEmpty(itemName)) throw new ArgumentNullException(nameof(itemName));
            if (!Enum.IsDefined(typeof(ItemType), itemType))
                throw new ArgumentException($"Invalid itemType. Valid values: {string.Join(", ", Enum.GetValues(typeof(ItemType)).Cast<int>())}.");

            var type = (ItemType)itemType;
            if (!_validator.Validate(itemContent, type))
                throw new ArgumentException($"Invalid {type} content.", nameof(itemContent));

            var item = new RepositoryItem { Content = itemContent, Type = type };
            if (!_storage.TryAdd(itemName, item))
                throw new InvalidOperationException($"Item '{itemName}' already registered.");
        }

        public string Retrieve(string itemName)
        {
            if (!_isInitialized) throw new InvalidOperationException("Initialize() must be called first.");
            if (string.IsNullOrEmpty(itemName)) throw new ArgumentNullException(nameof(itemName));
            if (!_storage.TryGet(itemName, out var item))
                throw new KeyNotFoundException($"Item '{itemName}' not found.");
            return item.Content;
        }

        public int GetType(string itemName)
        {
            if (!_isInitialized) throw new InvalidOperationException("Initialize() must be called first.");
            if (string.IsNullOrEmpty(itemName)) throw new ArgumentNullException(nameof(itemName));
            if (!_storage.TryGet(itemName, out var item))
                throw new KeyNotFoundException($"Item '{itemName}' not found.");
            return (int)item.Type;
        }

        public void Deregister(string itemName)
        {
            if (!_isInitialized) throw new InvalidOperationException("Initialize() must be called first.");
            if (string.IsNullOrEmpty(itemName)) throw new ArgumentNullException(nameof(itemName));
            if (!_storage.TryRemove(itemName, out _))
                throw new KeyNotFoundException($"Item '{itemName}' not found.");
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
        }
    }
}
