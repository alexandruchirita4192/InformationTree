using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers;

public class MoveNodeHandler : IRequestHandler<MoveNodeRequest, BaseResponse>
{
    private readonly IMediator _mediator;
    private readonly ITreeNodeSelectionCachingService _treeNodeSelectionCachingService;
    private readonly ITreeNodeToTreeNodeDataAdapter _treeNodeToTreeNodeDataAdapter;

    public MoveNodeHandler(
        IMediator mediator,
        ITreeNodeSelectionCachingService treeNodeSelectionCachingService,
        ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter)
    {
        _mediator = mediator;
        _treeNodeSelectionCachingService = treeNodeSelectionCachingService;
        _treeNodeToTreeNodeDataAdapter = treeNodeToTreeNodeDataAdapter;
    }

    public async Task<BaseResponse> Handle(MoveNodeRequest request, CancellationToken cancellationToken)
    {
        if (request.TreeView is not TreeView tv)
            return null;
        var oldSelectionObj = _treeNodeSelectionCachingService.GetOldSelectionFromCache();
        if (oldSelectionObj is not TreeNode oldSelection)
            return null;
        var currentSelectionObj = _treeNodeSelectionCachingService.GetCurrentSelectionFromCache();
        if (currentSelectionObj is not TreeNode currentSelection)
            return null;

        var parentSelected = currentSelection.Parent;
        if (parentSelected != null)
            parentSelected.Nodes.Remove(oldSelection);
        else
            tv.Nodes.Remove(oldSelection);

        var currentSelectionTagData = _treeNodeToTreeNodeDataAdapter.Adapt(currentSelection);
        if (currentSelection.Text.IsEmpty() &&
        currentSelectionTagData.IsEmptyData &&
        currentSelection.Parent == null &&
        currentSelection.Nodes.Count == 0)
            tv.Nodes.Add(oldSelection);
        else
            currentSelection.Nodes.Add(oldSelection);

        var setTreeStateRequest = new SetTreeStateRequest
        {
            TreeUnchanged = false
        };
        return await _mediator.Send(setTreeStateRequest, cancellationToken);
    }
}