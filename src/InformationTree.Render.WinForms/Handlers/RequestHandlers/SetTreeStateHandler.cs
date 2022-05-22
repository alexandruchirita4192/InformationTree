using System.Threading;
using System.Threading.Tasks;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Tree;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers
{
    public class SetTreeStateHandler : IRequestHandler<SetTreeStateRequest, BaseResponse>
    {
        public Task<BaseResponse> Handle(SetTreeStateRequest request, CancellationToken cancellationToken)
        {
            // TODO: Later move all these to some tree state set by a service in a cache for example

            if (request.IsSafeToSave.HasValue)
                TreeNodeHelper.IsSafeToSave = request.IsSafeToSave.Value;
            if (request.TreeNodeCounter.HasValue)
                TreeNodeHelper.TreeNodeCounter = request.TreeNodeCounter.Value;
            if (request.TreeUnchanged.HasValue)
                TreeNodeHelper.TreeUnchanged = request.TreeUnchanged.Value;
            if (request.TreeSaved.HasValue)
                TreeNodeHelper.TreeSaved = request.TreeSaved.Value;
            if (request.TreeSavedAt.HasValue)
                TreeNodeHelper.TreeSavedAt = request.TreeSavedAt.Value;
            if (request.ReadOnlyState.HasValue)
                TreeNodeHelper.ReadOnlyState = request.ReadOnlyState.Value;
            if (request.FileInfo != null)
                TreeNodeHelper.FileName = request.FileInfo.FileName;
            
            return Task.FromResult(new BaseResponse());
        }
    }
}