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

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers;

public class CalculatePercentageHandler : IRequestHandler<CalculatePercentageRequest, BaseResponse>
{
    private readonly IMediator _mediator;
    private readonly ITreeNodeToTreeNodeDataAdapter _treeNodeToTreeNodeDataAdapter;

    public CalculatePercentageHandler(
        IMediator mediator,
        ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter
        )
    {
        _mediator = mediator;
        _treeNodeToTreeNodeDataAdapter = treeNodeToTreeNodeDataAdapter;
    }

    public async Task<BaseResponse> Handle(CalculatePercentageRequest request, CancellationToken cancellationToken)
    {
        if (request.SelectedNode is not TreeNode selectedNode)
            return null;

        if (request.Direction == CalculatePercentageDirection.FromLeafsToSelectedNode)
        {
            var percentage = GetPercentageFromChildren(selectedNode)
                .ValidatePercentage();
            _treeNodeToTreeNodeDataAdapter.Adapt(selectedNode)
                .PercentCompleted = percentage;

            var setTreeStateRequest = new SetTreeStateRequest
            {
                TreeUnchanged = false
            };
            return await _mediator.Send(setTreeStateRequest, cancellationToken);
        }
        else if (request.Direction == CalculatePercentageDirection.FromSelectedNodeToLeafs)
        {
            var percentage = _treeNodeToTreeNodeDataAdapter.Adapt(selectedNode)
                .PercentCompleted
                .ValidatePercentage();
            
            SetPercentageToChildren(selectedNode, percentage);

            var setTreeStateRequest = new SetTreeStateRequest
            {
                TreeUnchanged = false
            };
            return await _mediator.Send(setTreeStateRequest, cancellationToken);
        }
        return new BaseResponse();
    }

    private decimal GetPercentageFromChildren(TreeNode topNode)
    {
        if (topNode == null)
            throw new ArgumentNullException(nameof(topNode));

        var sum = 0m;
        var nr = 0;
        foreach (TreeNode node in topNode.Nodes)
        {
            if (node != null && node.Nodes.Count > 0)
            {
                var procentCompleted = GetPercentageFromChildren(node);
                var procentCompletedForCurrentNode = _treeNodeToTreeNodeDataAdapter.Adapt(node)
                    .PercentCompleted
                    .ValidatePercentage();
                procentCompleted += procentCompletedForCurrentNode;

                sum += procentCompleted;
            }
            else
            {
                var procentCompleted = _treeNodeToTreeNodeDataAdapter.Adapt(node)
                    .PercentCompleted
                    .ValidatePercentage();
                sum += procentCompleted;
            }
            nr++;
        }

        return nr != 0 ? sum / nr : 0;
    }

    private void SetPercentageToChildren(TreeNode topNode, decimal percentage)
    {
        if (topNode == null)
            throw new ArgumentNullException(nameof(topNode));
        if (percentage < 0 || percentage > 100)
            throw new ArgumentOutOfRangeException(nameof(percentage));

        foreach (TreeNode node in topNode.Nodes)
        {
            if (node != null)
            {
                _treeNodeToTreeNodeDataAdapter.Adapt(node)
                    .PercentCompleted = percentage;
                if (node.Nodes.Count > 0)
                    SetPercentageToChildren(node, percentage);
            }
        }
    }
}