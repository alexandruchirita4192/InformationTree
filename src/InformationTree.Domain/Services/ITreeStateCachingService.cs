using InformationTree.Domain.Entities;

namespace InformationTree.Domain.Services
{
    public interface ITreeStateCachingService
    {
        void CacheTreeNodeState(TreeState treeNodeState);

        TreeState GetTreeNodeState();
    }
}