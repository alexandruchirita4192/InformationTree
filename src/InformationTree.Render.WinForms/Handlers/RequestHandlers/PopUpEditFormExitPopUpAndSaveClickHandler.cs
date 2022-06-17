using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class PopUpEditFormExitPopUpAndSaveClickHandler : IRequestHandler<PopUpEditFormExitPopUpAndSaveClickRequest, BaseResponse>
    {
        private readonly IMediator _mediator;

        public PopUpEditFormExitPopUpAndSaveClickHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<BaseResponse> Handle(PopUpEditFormExitPopUpAndSaveClickRequest request, CancellationToken cancellationToken)
        {
            if (request.ExitPopUpAndSaveTextBox is not TextBox tbExitPopUpAndSave)
                return null;
            if (request.Form is not Form form)
                return null;
            if (request.Sender == null)
                return null;

            if (ReferenceEquals(request.Sender, tbExitPopUpAndSave))
            {
                var formCloseRequest = new FormCloseRequest { Form = form };
                await _mediator.Send(formCloseRequest, cancellationToken);
            }

            return new BaseResponse();
        }
    }
}