using System.Threading;
using System.Threading.Tasks;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Tree;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class GetTreeStateHandler : IRequestHandler<GetTreeStateRequest, BaseResponse>
    {
        public Task<BaseResponse> Handle(GetTreeStateRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult<BaseResponse>(
                new GetTreeStateResponse
                {
                    IsSafeToSave = TreeNodeHelper.IsSafeToSave,
                    TreeNodeCounter = TreeNodeHelper.TreeNodeCounter,
                    TreeUnchanged = TreeNodeHelper.TreeUnchanged,
                    TreeSaved = TreeNodeHelper.TreeSaved,
                    TreeSavedAt = TreeNodeHelper.TreeSavedAt,
                    ReadOnlyState = TreeNodeHelper.ReadOnlyState,
                    FileName = TreeNodeHelper.FileName
                });
        }
    }
}