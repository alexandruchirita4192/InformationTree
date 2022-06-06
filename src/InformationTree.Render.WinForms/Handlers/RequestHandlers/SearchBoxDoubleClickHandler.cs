using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Forms;
using InformationTree.Render.WinForms.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class SearchBoxDoubleClickHandler : IRequestHandler<SearchBoxDoubleClickRequest, BaseResponse>
    {
        private readonly IMediator _mediator;

        private TextBox _tbSearchBox;
        private TreeView _tvTaskList;

        public SearchBoxDoubleClickHandler(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<BaseResponse> Handle(SearchBoxDoubleClickRequest request, CancellationToken cancellationToken)
        {
            if (request.Form is not MainForm mainForm)
                return Task.FromResult<BaseResponse>(null);
            if (request.SearchBoxTextBox is not TextBox tbSearchBox)
                return Task.FromResult<BaseResponse>(null);
            if (request.TreeView is not TreeView tvTaskList)
                return Task.FromResult<BaseResponse>(null);

            _tbSearchBox = tbSearchBox;
            _tvTaskList = tvTaskList;

            var form = new SearchForm(_mediator, tbSearchBox.Text);

            WinFormsApplication.CenterForm(form, mainForm);

            form.FormClosed += SearchForm_FormClosed;
            form.ShowDialog();

            return Task.FromResult(new BaseResponse());
        }

        private async void SearchForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is SearchForm form)
            {
                var textToFind = form.TextToFind;
                _tbSearchBox.Text = textToFind;

                var searchBoxKeyUpRequest = new SearchBoxKeyUpRequest
                {
                    SearchBoxTextBox = _tbSearchBox,
                    TreeView = _tvTaskList,
                    KeyData = (int)Keys.Enter
                };
                await _mediator.Send(searchBoxKeyUpRequest);
            }
        }
    }
}