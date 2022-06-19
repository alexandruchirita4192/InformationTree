using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormTreeViewKeyDownHandler : IRequestHandler<MainFormTreeViewKeyDownRequest, BaseResponse>
    {
        private readonly IMediator _mediator;
        private readonly ICachingService _cachingService;

        public MainFormTreeViewKeyDownHandler(
            IMediator mediator,
            ICachingService cachingService
            )
        {
            _mediator = mediator;
            _cachingService = cachingService;
        }

        public async Task<BaseResponse> Handle(MainFormTreeViewKeyDownRequest request, CancellationToken cancellationToken)
        {
            if (request.AfterSelectRequest == null)
                return null;
            if (request.AfterSelectRequest.Form is not Form form)
                return null;
            if (request.AfterSelectRequest.TreeView is not TreeView tvTaskList)
                return null;
            if (request.AfterSelectRequest.TaskNameTextBox is not TextBox tbTaskName)
                return null;
            if (request.SearchBoxTextBox is not TextBox tbSearchBox)
                return null;
            if (request.Sender == null)
                return null;
            if (request.ShowUntilNumberNumericUpDown is not NumericUpDown nudShowUntilNumber)
                return null;
            if (request.ShowFromNumberNumericUpDown is not NumericUpDown nudShowFromNumber)
                return null;

            var isControlKeyPressed = (request.KeyData & (int)Keys.Control) > 0;
            _cachingService.Set(Constants.CacheKeys.IsControlKeyPressed, isControlKeyPressed);

            if (request.KeyData == (int)(Keys.F & Keys.Control))
            {
                var searchBoxDoubleClickRequest = new SearchBoxDoubleClickRequest
                {
                    Form = form,
                    SearchBoxTextBox = tbSearchBox,
                    TreeView = tvTaskList,
                };
                return await _mediator.Send(searchBoxDoubleClickRequest, cancellationToken);
            }

            if (ReferenceEquals(request.Sender, tvTaskList))
            {
                if (request.KeyData == (int)Keys.Delete)
                {
                    if (tvTaskList.SelectedNode == null)
                        return null;

                    var treeViewDeleteRequest = new TreeViewDeleteRequest
                    {
                        TreeView = tvTaskList,
                        TaskNameText = tbTaskName.Text,
                        ShowUntilNumberNumericUpDown = nudShowUntilNumber,
                        ShowFromNumberNumericUpDown = nudShowFromNumber,
                        AfterSelectRequest = request.AfterSelectRequest
                    };
                    return await _mediator.Send(treeViewDeleteRequest, cancellationToken);
                }
                else if (request.KeyData == (int)Keys.Enter)
                {
                    var treeViewDoubleClickRequest = new TreeViewDoubleClickRequest
                    {
                        Form = form,
                        TreeView = tvTaskList,
                        TaskNameTextBox = tbTaskName,
                    };
                    return await _mediator.Send(treeViewDoubleClickRequest, cancellationToken);
                }
                else if (request.KeyData == (int)Keys.M)
                {
                    var moveNodeRequest = new MoveNodeRequest
                    {
                        TreeView = tvTaskList
                    };
                    return await _mediator.Send(moveNodeRequest, cancellationToken);
                }
            }

            return null;
        }
    }
}