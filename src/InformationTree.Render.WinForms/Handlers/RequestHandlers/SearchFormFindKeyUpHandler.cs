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
        
        public SearchFormFindKeyUpHandler(IPopUpService popUpService)
        {
            _popUpService = popUpService;
        }
        
        public Task<BaseResponse> Handle(SearchFormFindKeyUpRequest request, CancellationToken cancellationToken)
        {
            if (request.Form is not Form form)
                return Task.FromResult<BaseResponse>(null);
            if (request.FindTextBox is not TextBox tbFind)
                return Task.FromResult<BaseResponse>(null);
            var keyData = (Keys)request.KeyData;

            if (ReferenceEquals(tbFind, form) && keyData == Keys.Enter)
            {
                if (tbFind.Text.IsNotEmpty() && tbFind.Text.Length > 3)
                {
                    var foundTextWithoutEndLines = tbFind.Text.RemoveEndLines();
                    _popUpService.ShowMessage($"Searching for a node with text '{foundTextWithoutEndLines}'");

                    form.Close();
                }
            }

            if (keyData == Keys.Escape)
            {
                tbFind.Text = null;
                form.Close();
            }
            return Task.FromResult(new BaseResponse());
        }
    }
}