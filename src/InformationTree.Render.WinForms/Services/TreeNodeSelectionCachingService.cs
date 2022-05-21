using System;
using System.Windows.Forms;
using InformationTree.Domain.Services;

namespace InformationTree.Render.WinForms.Services
{
    public class TreeNodeSelectionCachingService : ITreeNodeSelectionCachingService
    {
        private TreeNode currentSelection;
        private TreeNode oldSelection;

        public void AddToCache(MarshalByRefObject treeNode)
        {
            if (treeNode is TreeNode node)
            {
                oldSelection = currentSelection;
                currentSelection = node;
            }
        }

        public MarshalByRefObject GetCurrentSelectionFromCache()
        {
            return currentSelection;
        }

        public MarshalByRefObject GetOldSelectionFromCache()
        {
            return oldSelection;
        }
    }
}