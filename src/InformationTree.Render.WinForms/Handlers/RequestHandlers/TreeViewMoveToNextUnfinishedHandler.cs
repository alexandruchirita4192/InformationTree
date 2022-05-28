using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.TextProcessing;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class TreeViewMoveToNextUnfinishedHandler : IRequestHandler<TreeViewCollapseAndRefreshRequest, BaseResponse>
    {
        public Task<BaseResponse> Handle(TreeViewCollapseAndRefreshRequest request, CancellationToken cancellationToken)
        {
            if (request.TreeView is not TreeView tvTaskList)
                return null;

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
                var completed = 0.0M;
                node.Text = TextProcessingHelper.GetTextAndProcentCompleted(node.Text, ref completed, true);

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
}