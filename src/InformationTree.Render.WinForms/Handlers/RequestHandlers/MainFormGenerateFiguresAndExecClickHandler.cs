using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormGenerateFiguresAndExecClickHandler : IRequestHandler<MainFormGenerateFiguresAndExecClickRequest, BaseResponse>
    {
        private readonly IMediator _mediator;

        public MainFormGenerateFiguresAndExecClickHandler(
            IMediator mediator
            )
        {
            _mediator = mediator;
        }

        public async Task<BaseResponse> Handle(MainFormGenerateFiguresAndExecClickRequest request, CancellationToken cancellationToken)
        {
            if (request.MainFormGenerateClickRequest == null)
                return null;
            if (request.MainFormGenerateClickRequest.CommandTextBox is not TextBox tbCommand)
                return null;

            await _mediator.Send(request.MainFormGenerateClickRequest, cancellationToken);

            // Lines are set after the previous MainFormGenerateClickRequest MediatR request, so this when we know if it will work
            if (tbCommand.Lines.Length <= 0)
                return null;

            var showCanvasPopUpRequest = new ShowCanvasPopUpRequest
            {
                FigureLines = tbCommand.Lines
            };
            return await _mediator.Send(showCanvasPopUpRequest, cancellationToken);
        }
    }
}