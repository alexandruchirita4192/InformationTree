using System.Collections;
using System.Collections.Concurrent;
using InformationTree.Domain.Services;

namespace InformationTree.Render.WinForms.Services
{
    public class ListCachingService : IListCachingService
    {
        private readonly ConcurrentDictionary<string, IList> _cache = new();

        public IList Get(string key)
        {
            return _cache.TryGetValue(key, out var value) ? value : null;
        }

        public void Set(string key, IList collection)
        {
            _cache.AddOrUpdate(key, collection, (k, v) => collection);
        }
    }
}