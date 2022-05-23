using InformationTree.Domain.Entities;
using InformationTree.Domain.Services;

namespace InformationTree.Render.WinForms.Services
{
    public class TreeStateCachingService : ITreeStateCachingService
    {
        private readonly object _lock = new();
        private TreeState _treeNodeState;

        public void CacheTreeNodeState(TreeState treeNodeState)
        {
            lock (_lock)
            {
                _treeNodeState = treeNodeState;
            }
        }

        public TreeState GetTreeNodeState()
        {
            lock (_lock)
            {
                return _treeNodeState;
            }
        }
    }
}