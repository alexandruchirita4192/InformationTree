using System.Threading;
using System.Threading.Tasks;
using InformationTree.Domain;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers;

public class SetTreeStateHandler : IRequestHandler<SetTreeStateRequest, BaseResponse>
{
    private readonly ICachingService _cachingService;

    public SetTreeStateHandler(ICachingService cachingService)
    {
        _cachingService = cachingService;
    }

    public Task<BaseResponse> Handle(SetTreeStateRequest request, CancellationToken cancellationToken)
    {
        var treeNodeState = _cachingService.Get<TreeState>(Constants.CacheKeys.TreeState)
            ?? new TreeState();

        if (request.IsSafeToSave.HasValue)
            treeNodeState.IsSafeToSave = request.IsSafeToSave.Value;
        if (request.TreeNodeCounter.HasValue)
            treeNodeState.TreeNodeCounter = request.TreeNodeCounter.Value;
        if (request.TreeUnchanged.HasValue)
            treeNodeState.TreeUnchanged = request.TreeUnchanged.Value;
        if (request.TreeSaved.HasValue)
            treeNodeState.TreeSaved = request.TreeSaved.Value;
        if (request.TreeSavedAt.HasValue)
            treeNodeState.TreeSavedAt = request.TreeSavedAt.Value;
        if (request.ReadOnlyState.HasValue)
            treeNodeState.ReadOnlyState = request.ReadOnlyState.Value;
        if (request.FileInformation != null)
            treeNodeState.FileName = request.FileInformation.FileName;
        if (request.TreeUnchangedValueChangeEventAdded)
            treeNodeState.TreeUnchangedValueChanged += (s, e) => request.InvokeTreeUnchangedValueChangeEvent(s, e);

        _cachingService.Set(Constants.CacheKeys.TreeState, treeNodeState);

        return Task.FromResult(new BaseResponse());
    }
}