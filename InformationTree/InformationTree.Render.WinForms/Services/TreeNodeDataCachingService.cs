using System;
using System.Collections.Concurrent;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Services;

namespace InformationTree.Render.WinForms.Services
{
    public class TreeNodeDataCachingService : ITreeNodeDataCachingService
    {
        private readonly ConcurrentDictionary<Guid, TreeNodeData> _cache = new();

        public Guid AddToCache(TreeNodeData treeNodeData)
        {
            if (treeNodeData == null)
                throw new ArgumentNullException(nameof(treeNodeData));

            var newGuid = Guid.NewGuid();
            return _cache.TryAdd(newGuid, treeNodeData) ? newGuid : Guid.Empty;
        }

        public TreeNodeData GetFromCache(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty", nameof(id));
            return _cache.TryGetValue(id, out var treeNodeData) ? treeNodeData : null;
        }
    }
}