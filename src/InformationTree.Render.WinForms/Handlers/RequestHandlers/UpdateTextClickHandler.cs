using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Render.WinForms.Extensions;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers;

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
        if (request.AfterSelectRequest == null)
            return null;
        if (request.AfterSelectRequest.TreeView is not TreeView tvTaskList)
            return null;
        if (request.AfterSelectRequest.TaskNameTextBox is not TextBox tbTaskName)
            return null;
        if (request.AfterSelectRequest.LinkTextBox is not TextBox tbLink)
            return null;
        if (request.AfterSelectRequest.UrgencyNumericUpDown is not NumericUpDown nudUrgency)
            return null;
        if (request.AfterSelectRequest.CategoryTextBox is not TextBox tbCategory)
            return null;
        if (request.AfterSelectRequest.IsStartupAlertCheckBox is not CheckBox cbIsStartupAlert)
            return null;

        if (tvTaskList.SelectedNode == null)
            return null;
        var selectedNode = _treeNodeToTreeNodeDataAdapter.Adapt(tvTaskList.SelectedNode);
        if (selectedNode == null)
            return null;

        selectedNode.Urgency = (int)nudUrgency.Value;
        selectedNode.Link = tbLink.Text;
        selectedNode.Category = tbCategory.Text;
        selectedNode.IsStartupAlert = cbIsStartupAlert.Checked;
        selectedNode.PercentCompleted = request.AfterSelectRequest.TaskPercentCompleted;
        selectedNode.LastChangeDate = DateTime.Now;
        selectedNode.Text = tbTaskName.Text;

        await _mediator.Send(request.AfterSelectRequest, cancellationToken);

        var setTreeStateRequest = new SetTreeStateRequest
        {
            TreeUnchanged = false
        };
        await _mediator.Send(setTreeStateRequest, cancellationToken);

        tvTaskList.InvokeWrapper(tvTaskList =>
        {
            var treeNode = _treeNodeDataToTreeNodeAdapter.Adapt(selectedNode);
            if (treeNode is TreeNode treeNodeControl)
                tvTaskList.SelectedNode.Copy(treeNodeControl, _treeNodeToTreeNodeDataAdapter, false);
        });

        return new BaseResponse();
    }
}