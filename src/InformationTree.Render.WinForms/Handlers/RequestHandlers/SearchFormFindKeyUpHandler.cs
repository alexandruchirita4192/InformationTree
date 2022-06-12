using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    internal class SearchFormFindKeyUpHandler : IRequestHandler<SearchFormFindKeyUpRequest, BaseResponse>
    {
        private readonly IPopUpService _popUpService;
        private readonly IMediator _mediator;

        public SearchFormFindKeyUpHandler(
            IPopUpService popUpService,
            IMediator mediator)
        {
            _popUpService = popUpService;
            _mediator = mediator;
        }
        
        public async Task<BaseResponse> Handle(SearchFormFindKeyUpRequest request, CancellationToken cancellationToken)
        {
            if (request.Form is not Form form)
                return null;
            if (request.FindTextBox is not TextBox tbFind)
                return null;
            
            var formCloseRequest = new FormCloseRequest
            {
                Form = form
            };
            
            if (ReferenceEquals(tbFind, form) && request.KeyData == (int)Keys.Enter)
            {
                if (tbFind.Text.IsNotEmpty() && tbFind.Text.Length > 3)
                {
                    var foundTextWithoutEndLines = tbFind.Text.RemoveEndLines();
                    _popUpService.ShowMessage($"Searching for a node with text '{foundTextWithoutEndLines}'");

                    await _mediator.Send(formCloseRequest, cancellationToken);
                }
            }

            if (request.KeyData == (int)Keys.Escape)
            {
                tbFind.Text = null;
                
                await _mediator.Send(formCloseRequest, cancellationToken);
            }
            return new BaseResponse();
        }
    }
}