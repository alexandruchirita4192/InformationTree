using System;
using System.Windows.Forms;
using InformationTree.Domain.Services;

namespace InformationTree.Render.WinForms.Services
{
    public class TreeNodeSelectionCachingService : ITreeNodeSelectionCachingService
    {
        private readonly object _lock = new();
        private TreeNode currentSelection;
        private TreeNode oldSelection;

        public void AddToCache(MarshalByRefObject treeNode)
        {
            lock (_lock)
            {
                if (treeNode is TreeNode node
                && !ReferenceEquals(currentSelection, node))
                {
                    oldSelection = currentSelection;
                    currentSelection = node;
                }
            }
        }

        public MarshalByRefObject GetCurrentSelectionFromCache()
        {
            lock (_lock)
            {
                return currentSelection;
            }
        }

        public MarshalByRefObject GetOldSelectionFromCache()
        {
            lock (_lock)
            {
                return oldSelection;
            }
        }
    }
}