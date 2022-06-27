using System;

namespace InformationTree.Domain.Services
{
    public interface ITreeNodeSelectionCachingService
    {
        void AddToCache(MarshalByRefObject treeNode);

        MarshalByRefObject GetCurrentSelectionFromCache();

        MarshalByRefObject GetOldSelectionFromCache();
    }
}