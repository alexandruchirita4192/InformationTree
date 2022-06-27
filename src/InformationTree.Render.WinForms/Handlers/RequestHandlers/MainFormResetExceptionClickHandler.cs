using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormResetExceptionClickHandler : IRequestHandler<MainFormResetExceptionClickRequest, BaseResponse>
    {
        private readonly IMediator _mediator;

        public MainFormResetExceptionClickHandler(
            IMediator mediator)
        {
            _mediator = mediator;
        }
        
        public async Task<BaseResponse> Handle(MainFormResetExceptionClickRequest request, CancellationToken cancellationToken)
        {
            if (request.ResetExceptionButton is not Button btnResetException)
                return null;

            var setTreeStateRequest = new SetTreeStateRequest
            {
                IsSafeToSave = true
            };
            await _mediator.Send(setTreeStateRequest, cancellationToken);

            btnResetException.Enabled = false;
            btnResetException.BackColor = Constants.Colors.DefaultBackGroundColor;
            btnResetException.ForeColor = Constants.Colors.DefaultForeGroundColor;
            btnResetException.Text = "Reset exception";
            
            return new BaseResponse();
        }
    }
}