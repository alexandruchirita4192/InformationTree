using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Render.WinForms.Extensions;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers;

public class ShowTreeFilteredByRangeHandler : IRequestHandler<ShowTreeFilteredByRangeRequest, BaseResponse>
{
    private readonly IMediator _mediator;
    private readonly ITreeNodeToTreeNodeDataAdapter _treeNodeToTreeNodeDataAdapter;
    private readonly ICachingService _cachingService;

    public ShowTreeFilteredByRangeHandler(
        IMediator mediator,
        ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter,
        ICachingService cachingService)
    {
        _mediator = mediator;
        _treeNodeToTreeNodeDataAdapter = treeNodeToTreeNodeDataAdapter;
        _cachingService = cachingService;
    }

    public async Task<BaseResponse> Handle(ShowTreeFilteredByRangeRequest request, CancellationToken cancellationToken)
    {
        if (request.TreeView is not TreeView tvTaskList)
            return null;
        if (request.ControlsToEnableForFiltered == null || request.ControlsToEnableForFiltered.Count == 0)
            return null;
        if (request.ControlsToEnableForNotFiltered == null || request.ControlsToEnableForNotFiltered.Count == 0)
            return null;

        var controlsToEnableForFiltered = new List<Control>();
        foreach (var component in request.ControlsToEnableForFiltered)
        {
            if (component is not Control control)
                return null;
            controlsToEnableForFiltered.Add(control);
        }
        var controlsToEnableForNotFiltered = new List<Control>();
        foreach (var component in request.ControlsToEnableForNotFiltered)
        {
            if (component is not Control control)
                return null;
            controlsToEnableForNotFiltered.Add(control);
        }
        if (await _mediator.Send(new GetTreeStateRequest(), cancellationToken) is not GetTreeStateResponse getTreeStateResponse)
            return null;
        if (IsProperState(request.FilterType, getTreeStateResponse.ReadOnlyState) == false)
            return null;

        await ShowTasksBasedOn(
            tvTaskList,
            request.Max,
            request.Min,
            request.FilterType,
            cancellationToken);
        SetComponentsByFilterType(request.FilterType, controlsToEnableForFiltered, controlsToEnableForNotFiltered);
        return new BaseResponse();
    }

    private static bool IsProperState(CopyNodeFilterType filterType, bool readOnlyState)
    {
        return (filterType == CopyNodeFilterType.NoFilter && readOnlyState == true)
            || (filterType == CopyNodeFilterType.FilterByAddedNumber && readOnlyState == false)
            || (filterType == CopyNodeFilterType.FilterByUrgency && readOnlyState == false);
    }

    private static void SetComponentsByFilterType(CopyNodeFilterType filterType, List<Control> controlsToEnableForFiltered, List<Control> controlsToEnableForNotFiltered)
    {
        // Enable controls that restore state of tree only for the filtered state of the tree (expected btnShowAll)
        var isFiltered = filterType != CopyNodeFilterType.NoFilter;
        foreach (var control in controlsToEnableForFiltered)
        {
            control.Enabled = isFiltered;
        }

        // Enable editing of task in any way only for the not filtered state of the tree
        var isNotFiltered = filterType == CopyNodeFilterType.NoFilter;
        foreach (var control in controlsToEnableForNotFiltered)
        {
            control.Enabled = isNotFiltered;
        }
    }

    private async Task ShowTasksBasedOn(
        TreeView tvTaskList,
        decimal filterLowerThan,
        decimal filterHigherThan,
        CopyNodeFilterType filterType,
        CancellationToken cancellationToken)
    {
        if (_cachingService.Get(Constants.CacheKeys.NodesList) is not TreeNodeCollection nodes)
        {
            nodes = new TreeNode().Nodes;
            _cachingService.Set(Constants.CacheKeys.NodesList, nodes);
        }

        var isFiltered = filterType != CopyNodeFilterType.NoFilter;
        if (isFiltered)
        {
            // Clear cache
            nodes.Clear();

            // Copy tree to cache to save current tree state
            nodes.Copy(tvTaskList.Nodes, _treeNodeToTreeNodeDataAdapter, null, null, CopyNodeFilterType.NoFilter);
            _cachingService.Set(Constants.CacheKeys.NodesList, nodes);

            // Update tree view with filtered nodes
            tvTaskList.Nodes.Clear();
            tvTaskList.Nodes.Copy(nodes, _treeNodeToTreeNodeDataAdapter, (int)filterHigherThan, (int)filterLowerThan, filterType);
        }
        else
        {
            // Clear tree view nodes
            tvTaskList.Nodes.Clear();

            // Copy tree back from cache to tree view to restore current tree state
            tvTaskList.Nodes.Copy(nodes, _treeNodeToTreeNodeDataAdapter, null, null, CopyNodeFilterType.NoFilter);
        }

        // Update read only state that keeps track of whether the tree is filtered or not
        var setTreeStateRequest = new SetTreeStateRequest
        {
            ReadOnlyState = isFiltered
        };
        await _mediator.Send(setTreeStateRequest, cancellationToken);
    }
}