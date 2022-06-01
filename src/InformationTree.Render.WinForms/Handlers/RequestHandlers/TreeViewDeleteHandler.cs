using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
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
            var deletedItemsWithName = InternalParseToDelete(tvTaskList, selectedTask, taskName, true);

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

            // TODO: After checking throughly delete zombie code and add a proper comment specifying exactly which event is called on "control delete"
            //TreeNodeHelper.TreeUnchanged = false; // on control delete is added too
            return new BaseResponse();
        }

        // TODO: Maybe refactor and join the following 2 methods (they had same name, have same parameters, have some common logic)
        private int ParseToDelete(TreeView tvTaskList, TreeNode selectedTask, string taskName, bool fakeDelete)
        {
            var deletedItemsWithName = 0;

            if (selectedTask != null && selectedTask.Text.Equals(taskName))
                deletedItemsWithName = InternalParseToDelete(tvTaskList, selectedTask, taskName, fakeDelete);
            else
                foreach (TreeNode node in tvTaskList.Nodes)
                {
                    if (node != null && node.Text.Equals(taskName))
                    {
                        node.Nodes.Clear();
                        tvTaskList.Nodes.Remove(node);
                    }
                    if (node.Nodes.Count > 0)
                        deletedItemsWithName = InternalParseToDelete(tvTaskList, node, taskName, fakeDelete);
                }

            return deletedItemsWithName;
        }

        private int InternalParseToDelete(TreeView tv, TreeNode topNode, string nodeNameToDelete, bool fakeDelete = true)
        {
            if (tv == null)
                throw new ArgumentNullException(nameof(tv));
            if (topNode == null)
                throw new ArgumentNullException(nameof(topNode));
            if (nodeNameToDelete.IsEmpty())
                throw new ArgumentNullException(nameof(nodeNameToDelete));

            int ret = 0;
            if (topNode.Text.Equals(nodeNameToDelete))
            {
                if (!fakeDelete)
                {
                    topNode.Nodes.Clear();
                    tv.Nodes.Remove(topNode);
                }
                ret++;
            }
            else
                foreach (TreeNode node in topNode.Nodes)
                {
                    if (node != null && node.Text.Equals(nodeNameToDelete))
                    {
                        if (!fakeDelete)
                        {
                            node.Nodes.Clear();
                            tv.Nodes.Remove(node);
                        }

                        ret++;
                    }
                    if (node != null && node.Nodes.Count > 0)
                        ret += InternalParseToDelete(tv, node, nodeNameToDelete);
                }
            return ret;
        }
    }
}