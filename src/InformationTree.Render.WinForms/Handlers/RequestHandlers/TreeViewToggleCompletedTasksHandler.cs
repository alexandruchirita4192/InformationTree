using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers;

public class TreeViewToggleCompletedTasksHandler : IRequestHandler<TreeViewToggleCompletedTasksRequest, BaseResponse>
{
    private readonly ITreeNodeToTreeNodeDataAdapter _treeNodeToTreeNodeDataAdapter;

    public TreeViewToggleCompletedTasksHandler(ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter)
    {
        _treeNodeToTreeNodeDataAdapter = treeNodeToTreeNodeDataAdapter;
    }

    public Task<BaseResponse> Handle(TreeViewToggleCompletedTasksRequest request, CancellationToken cancellationToken)
    {
        if (request.TreeView is not TreeView tvTaskList)
            return Task.FromResult<BaseResponse>(null);

        var completedTasksAreHidden = TasksCompleteAreHidden(tvTaskList) ?? false;
        ToggleCompletedTasks(!completedTasksAreHidden, tvTaskList.Nodes);

        return Task.FromResult(new BaseResponse());
    }

    private bool? TasksCompleteAreHidden(TreeView tv)
    {
        if (tv.Nodes.Count > 0)
        {
            foreach (TreeNode node in tv.Nodes)
            {
                if (node == null)
                    continue;
                
                var completed = _treeNodeToTreeNodeDataAdapter.Adapt(node)
                    .PercentCompleted
                    .ValidatePercentage();

                if (completed == 100)
                {
                    if (node.ForeColor.Name == Constants.Colors.DefaultBackGroundColor.ToString())
                        return true;
                    else
                        return false;
                }

                if (node.Nodes.Count > 0)
                {
                    var ret = TasksCompleteAreHidden(tv);
                    if (ret.HasValue)
                        return ret;
                    // else continue;
                }
            }
        }
        return null;
    }

    private void ToggleCompletedTasks(bool toggleCompletedTasksAreHidden, TreeNodeCollection nodes)
    {
        if (nodes.Count > 0)
        {
            var foreColor = toggleCompletedTasksAreHidden ? Constants.Colors.DefaultForeGroundColor : Constants.Colors.DefaultBackGroundColor;

            foreach (TreeNode node in nodes)
            {
                if (node == null)
                    continue;
                
                var completed = _treeNodeToTreeNodeDataAdapter.Adapt(node)
                    .PercentCompleted
                    .ValidatePercentage();

                if (completed == 100)
                    node.ForeColor = foreColor;

                if (node.Nodes.Count > 0)
                    ToggleCompletedTasks(toggleCompletedTasksAreHidden, node.Nodes);
            }
        }
    }
}