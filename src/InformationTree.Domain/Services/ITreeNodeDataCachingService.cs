using System;
using InformationTree.Domain.Entities;

namespace InformationTree.Domain.Services
{
    public interface ITreeNodeDataCachingService
    {
        Guid AddToCache(TreeNodeData treeNodeData);

        TreeNodeData GetFromCache(Guid id);
    }
}