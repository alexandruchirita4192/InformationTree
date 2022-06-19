using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Render.WinForms.Extensions;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormDoNotSaveClickHandler : IRequestHandler<MainFormDoNotSaveClickRequest, BaseResponse>
    {
        private readonly IMediator _mediator;

        public MainFormDoNotSaveClickHandler(
            IMediator mediator
            )
        {
            _mediator = mediator;
        }

        public async Task<BaseResponse> Handle(MainFormDoNotSaveClickRequest request, CancellationToken cancellationToken)
        {
            if (request.ResetExceptionButton is not Button btnResetException)
                return null;

            var setTreeStateRequest = new SetTreeStateRequest
            {
                IsSafeToSave = false
            };
            await _mediator.Send(setTreeStateRequest, cancellationToken);

            btnResetException.InvokeWrapper(btnResetException => btnResetException.Enabled = true);

            return new BaseResponse();
        }
    }
}