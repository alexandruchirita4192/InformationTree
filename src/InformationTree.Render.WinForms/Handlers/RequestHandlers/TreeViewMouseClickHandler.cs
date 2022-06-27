using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers;

public class TreeViewMouseClickHandler : IRequestHandler<TreeViewMouseClickRequest, BaseResponse>
{
    public Task<BaseResponse> Handle(TreeViewMouseClickRequest request, CancellationToken cancellationToken)
    {
        if (request.IsControlPressed == false)
            return Task.FromResult<BaseResponse>(null);
        if (request.TreeView is not TreeView tvTaskList)
            return Task.FromResult<BaseResponse>(null);

        var deltaFloat = (float)request.MouseDelta;
        var changedSize = deltaFloat / 120f;

        UpdateSizeOfTreeNodes(tvTaskList.Nodes, changedSize);

        return Task.FromResult(new BaseResponse());
    }

    /// <summary>
    /// Changes the size of a collection of nodes recursively.
    /// </summary>
    /// <param name="treeNodeCollection">Collection of nodes.</param>
    /// <param name="changedSize">Font size changed to all the nodes (added or substracted from nodes size).</param>
    private void UpdateSizeOfTreeNodes(TreeNodeCollection treeNodeCollection, float changedSize)
    {
        if (treeNodeCollection.Count == 0)
            return;
        
        foreach (TreeNode node in treeNodeCollection)
        {
            node.NodeFont = new Font(node.NodeFont.FontFamily, node.NodeFont.Size + changedSize, node.NodeFont.Style);

            // Change size of children recursively too
            foreach (TreeNode childNode in node.Nodes)
                UpdateSizeOfTreeNodes(childNode.Nodes, changedSize);
        }
    }
}