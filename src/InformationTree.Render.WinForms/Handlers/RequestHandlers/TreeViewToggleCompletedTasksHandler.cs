using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.TextProcessing;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class TreeViewToggleCompletedTasksHandler : IRequestHandler<TreeViewToggleCompletedTasksRequest, BaseResponse>
    {
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
                    var completed = 0.0M;
                    node.Text = TextProcessingHelper.GetTextAndProcentCompleted(node.Text, ref completed, true);

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
                    var completed = 0.0M;
                    node.Text = TextProcessingHelper.GetTextAndProcentCompleted(node.Text, ref completed, true);

                    if (completed == 100)
                        node.ForeColor = foreColor;

                    if (node.Nodes.Count > 0)
                        ToggleCompletedTasks(toggleCompletedTasksAreHidden, node.Nodes);
                }
            }
        }
    }
}