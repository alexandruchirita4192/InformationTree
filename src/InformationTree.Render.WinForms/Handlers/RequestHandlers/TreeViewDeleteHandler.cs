using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers;

public class TreeViewDeleteHandler : IRequestHandler<TreeViewDeleteRequest, BaseResponse>
{
    private readonly IMediator _mediator;
    private readonly IPopUpService _popUpService;

    public TreeViewDeleteHandler(
        IMediator mediator,
        IPopUpService popUpService)
    {
        _mediator = mediator;
        _popUpService = popUpService;
    }

    public async Task<BaseResponse> Handle(TreeViewDeleteRequest request, CancellationToken cancellationToken)
    {
        if (request.TreeView is not TreeView tvTaskList)
            return null;

        var selectedTask = tvTaskList.SelectedNode;
        var taskName = request.TaskNameText;
        var deletedItemsWithName = ParseToDelete(tvTaskList, selectedTask, taskName, true);

        if (deletedItemsWithName != 0)
        {
            var itemOrItems = deletedItemsWithName == 1 ? " item" : " items";
            var messageBoxText = $"Delete {deletedItemsWithName} {itemOrItems} with name {taskName}?";
            var messageBoxCaption = $"Delete";

            var result = _popUpService.ShowQuestion(messageBoxText, messageBoxCaption);
            if (result == PopUpResult.Yes)
            {
                ParseToDelete(tvTaskList, selectedTask, taskName, false);

                var treeViewNoTaskRequest = new TreeViewNoTaskRequest
                {
                    TreeView = tvTaskList,
                    AfterSelectRequest = request.AfterSelectRequest
                };
                await _mediator.Send(treeViewNoTaskRequest, cancellationToken);

                var updateNodeCountRequest = new UpdateNodeCountRequest
                {
                    TreeView = tvTaskList,
                    ShowUntilNumberNumericUpDown = request.ShowUntilNumberNumericUpDown,
                    ShowFromNumberNumericUpDown = request.ShowFromNumberNumericUpDown,
                };
                await _mediator.Send(updateNodeCountRequest, cancellationToken);
            }
        }

        // on control delete TreeUnchanged is set false too (tvTaskList_ControlRemoved)
        return new BaseResponse();
    }

    private int ParseToDelete(TreeView tvTaskList, TreeNode selectedTask, string taskName, bool fakeDelete)
    {
        var deletedItemsWithName = 0;

        if (selectedTask != null && selectedTask.Text.Equals(taskName))
        {
            if (!fakeDelete)
            {
                selectedTask.Nodes.Clear();
                tvTaskList.Nodes.Remove(selectedTask);
            }
            deletedItemsWithName++;
        }
        else
            foreach (TreeNode node in tvTaskList.Nodes)
            {
                if (node != null && node.Text.Equals(taskName))
                {
                    if (!fakeDelete)
                    {
                        node.Nodes.Clear();
                        tvTaskList.Nodes.Remove(node);
                    }
                    deletedItemsWithName++;
                }
                if (node.Nodes.Count > 0)
                    deletedItemsWithName += ParseToDelete(tvTaskList, node, taskName, fakeDelete);
            }

        return deletedItemsWithName;
    }
}