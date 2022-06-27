using System;
using InformationTree.Domain.Entities;

namespace InformationTree.Domain.Services
{
    public interface ITreeNodeToTreeNodeDataAdapter
    {
        TreeNodeData Adapt(MarshalByRefObject treeNode);
        TreeNodeData AdaptTreeView(MarshalByRefObject treeView);
    }
}