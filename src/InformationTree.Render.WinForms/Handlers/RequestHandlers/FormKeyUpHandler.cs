using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class FormKeyUpHandler : IRequestHandler<FormKeyUpRequest, BaseResponse>
    {
        private readonly IMediator _mediator;

        public FormKeyUpHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<BaseResponse> Handle(FormKeyUpRequest request, CancellationToken cancellationToken)
        {
            if (request.Form is not Form form)
                return null;

            if (request.KeyData == (int)Keys.Escape)
            {
                var formCloseRequest = new FormCloseRequest
                {
                    Form = form
                };
                await _mediator.Send(formCloseRequest, cancellationToken);
            }

            return new BaseResponse();
        }
    }
}