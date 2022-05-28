using System;
using System.Threading;
using System.Threading.Tasks;
using InformationTree.Domain;
using InformationTree.Domain.Entities;
using InformationTree.Domain.EventArguments;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class GetTreeStateHandler : IRequestHandler<GetTreeStateRequest, BaseResponse>
    {
        private readonly ICachingService _cachingService;

        public GetTreeStateHandler(ICachingService cachingService)
        {
            _cachingService = cachingService;
        }

        public Task<BaseResponse> Handle(GetTreeStateRequest request, CancellationToken cancellationToken)
        {
            var treeNodeState = _cachingService.Get<TreeState>(Constants.CacheKeys.TreeState)
                ?? new TreeState();

            var getTreeStateResponse =
                new GetTreeStateResponse
                {
                    IsSafeToSave = treeNodeState.IsSafeToSave,
                    TreeNodeCounter = treeNodeState.TreeNodeCounter,
                    TreeUnchanged = treeNodeState.TreeUnchanged,
                    TreeSaved = treeNodeState.TreeSaved,
                    TreeSavedAt = treeNodeState.TreeSavedAt,
                    ReadOnlyState = treeNodeState.ReadOnlyState,
                    FileName = treeNodeState.FileName
                };

            var eventHandler = new EventHandler<ValueChangedEventArgs<bool>>(
                (s, e) => treeNodeState?.InvokeTreeUnchangedValueChangedEventHandler(s, e));

            getTreeStateResponse.TreeUnchangedValueChanged += eventHandler;

            return Task.FromResult<BaseResponse>(getTreeStateResponse);
        }
    }
}