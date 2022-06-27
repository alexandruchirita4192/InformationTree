using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers;

public class TreeViewMoveToNextUnfinishedHandler : IRequestHandler<TreeViewCollapseAndRefreshRequest, BaseResponse>
{
    private readonly ITreeNodeToTreeNodeDataAdapter _treeNodeToTreeNodeDataAdapter;
    public TreeViewMoveToNextUnfinishedHandler(ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter)
    {
        _treeNodeToTreeNodeDataAdapter = treeNodeToTreeNodeDataAdapter;
    }

    public Task<BaseResponse> Handle(TreeViewCollapseAndRefreshRequest request, CancellationToken cancellationToken)
    {
        if (request.TreeView is not TreeView tvTaskList)
            return Task.FromResult<BaseResponse>(null);

        var selectedNode = tvTaskList.SelectedNode;
        if (tvTaskList.Nodes.Count > 0)
        {
            if (selectedNode == null && tvTaskList.Nodes.Count > 0)
                selectedNode = tvTaskList.Nodes[0];

            MoveToNextUnfinishedNode(tvTaskList, selectedNode);
        }

        return Task.FromResult(new BaseResponse());
    }

    private void MoveToNextUnfinishedNode(TreeView tv, TreeNode currentNode)
    {
        if (tv == null || currentNode == null)
            return;

        foreach (TreeNode node in currentNode.Nodes)
        {
            var completed = _treeNodeToTreeNodeDataAdapter.Adapt(currentNode)
                .PercentCompleted
                .ValidatePercentage();
            
            if (completed != 100 && tv.SelectedNode != node)
            {
                tv.SelectedNode = node;
                return;
            }
        }

        if (currentNode.Parent != null)
        {
            var foundCurrentNode = false;
            foreach (TreeNode node in currentNode.Parent.Nodes)
            {
                if (foundCurrentNode && !ReferenceEquals(node, currentNode))
                {
                    MoveToNextUnfinishedNode(tv, node);
                    return;
                }

                if (ReferenceEquals(node, currentNode))
                    foundCurrentNode = true;
            }

            MoveToNextUnfinishedNode(tv, currentNode.Parent);
        }
    }
}