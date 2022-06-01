using System;
using InformationTree.Domain.Entities;

namespace InformationTree.Domain.Services
{
    public interface ITreeNodeDataToTreeNodeAdapter
    {
        MarshalByRefObject Adapt(TreeNodeData treeNodeData, bool includeChildren = true);
        void AdaptToTreeView(TreeNodeData treeNodeData, MarshalByRefObject treeView, bool includeChildren = true);
    }
}