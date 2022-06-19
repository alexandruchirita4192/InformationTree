using System.Threading;
using System.Threading.Tasks;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormUnchangedClickHandler : IRequestHandler<MainFormUnchangedClickRequest, BaseResponse>
    {
        private readonly IMediator _mediator;

        public MainFormUnchangedClickHandler(
            IMediator mediator
            )
        {
            _mediator = mediator;
        }

        public async Task<BaseResponse> Handle(MainFormUnchangedClickRequest request, CancellationToken cancellationToken)
        {
            if (await _mediator.Send(new GetTreeStateRequest(), cancellationToken) is not GetTreeStateResponse getTreeStateResponse)
                return null;

            var setTreeStateRequest = new SetTreeStateRequest
            {
                TreeUnchanged = !getTreeStateResponse.TreeUnchanged
            };
            return await _mediator.Send(setTreeStateRequest, cancellationToken);
        }
    }
}