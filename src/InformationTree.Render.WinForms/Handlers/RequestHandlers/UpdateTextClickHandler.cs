using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Render.WinForms.Extensions;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class UpdateTextClickHandler : IRequestHandler<UpdateTextClickRequest, BaseResponse>
    {
        private readonly IMediator _mediator;
        private readonly ITreeNodeToTreeNodeDataAdapter _treeNodeToTreeNodeDataAdapter;
        private readonly ITreeNodeDataToTreeNodeAdapter _treeNodeDataToTreeNodeAdapter;

        public UpdateTextClickHandler(
            IMediator mediator,
            ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter,
            ITreeNodeDataToTreeNodeAdapter treeNodeDataToTreeNodeAdapter)
        {
            _mediator = mediator;
            _treeNodeToTreeNodeDataAdapter = treeNodeToTreeNodeDataAdapter;
            _treeNodeDataToTreeNodeAdapter = treeNodeDataToTreeNodeAdapter;
        }

        public async Task<BaseResponse> Handle(UpdateTextClickRequest request, CancellationToken cancellationToken)
        {
            var selectedNode = request.SelectedNode;
            if (selectedNode == null)
                return null;

            selectedNode.Urgency = request.Urgency;
            selectedNode.Link = request.Link;
            selectedNode.Category = request.Category;
            selectedNode.IsStartupAlert = request.IsStartupAlert;
            selectedNode.PercentCompleted = request.TaskPercentCompleted;
            selectedNode.LastChangeDate = DateTime.Now;
            selectedNode.Text = request.TaskName;

            await _mediator.Send(request.AfterSelectRequest, cancellationToken);

            var setTreeStateRequest = new SetTreeStateRequest
            {
                TreeUnchanged = false
            };
            await _mediator.Send(setTreeStateRequest, cancellationToken);

            if (request.TaskListTreeView is TreeView tvTaskList)
            {
                tvTaskList.InvokeWrapper(tvTaskList =>
                {
                    var treeNode = _treeNodeDataToTreeNodeAdapter.Adapt(selectedNode);
                    if (treeNode is TreeNode treeNodeControl)
                        tvTaskList.SelectedNode.Copy(treeNodeControl, _treeNodeToTreeNodeDataAdapter, false);
                });
            }

            return new BaseResponse();
        }
    }
}