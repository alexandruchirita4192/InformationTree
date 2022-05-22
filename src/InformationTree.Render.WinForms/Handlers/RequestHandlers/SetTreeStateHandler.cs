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
            if (request.File != null)
                TreeNodeHelper.FileName = request.File.FileName;

            return Task.FromResult(new BaseResponse());
        }
    }
}