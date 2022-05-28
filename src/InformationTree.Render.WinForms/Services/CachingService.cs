using System.Collections.Concurrent;
using InformationTree.Domain.Services;

namespace InformationTree.Render.WinForms.Services
{
    public class CachingService : ICachingService
    {
        private readonly ConcurrentDictionary<string, object> _cache = new();

        public object Get(string key)
        {
            return _cache.TryGetValue(key, out var value) ? value : null;
        }

        public TItem Get<TItem>(string key)
        {
            return Get(key) is TItem item ? item : default;
        }

        public void Set<T>(string key, T item)
        {
            _cache.AddOrUpdate(key, item, (k, v) => item);
        }
    }
}