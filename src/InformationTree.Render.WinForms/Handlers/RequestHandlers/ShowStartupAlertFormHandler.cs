using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Forms;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class ShowStartupAlertFormHandler : IRequestHandler<ShowStartupAlertFormRequest, BaseResponse>
    {
        private readonly IImportTreeFromXmlService _importTreeFromXmlService;
        private readonly IMediator _mediator;
        private readonly ITreeNodeDataToTreeNodeAdapter _treeNodeDataToTreeNodeAdapter;
        private readonly ITreeNodeToTreeNodeDataAdapter _treeNodeToTreeNodeDataAdapter;
        private TreeView _tvTaskList;
        private TextBox _tbSearchBox;

        public ShowStartupAlertFormHandler(
            IImportTreeFromXmlService importTreeFromXmlService,
            IMediator mediator,
            ITreeNodeDataToTreeNodeAdapter treeNodeDataToTreeNodeAdapter,
            ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter)
        {
            _importTreeFromXmlService = importTreeFromXmlService;
            _mediator = mediator;
            _treeNodeDataToTreeNodeAdapter = treeNodeDataToTreeNodeAdapter;
            _treeNodeToTreeNodeDataAdapter = treeNodeToTreeNodeDataAdapter;
        }

        public Task<BaseResponse> Handle(ShowStartupAlertFormRequest request, CancellationToken cancellationToken)
        {
            if (request.TreeView is not TreeView tvTaskList)
                return Task.FromResult<BaseResponse>(null);
            if (request.SearchBoxTextBox is not TextBox tbSearchBox)
                return Task.FromResult<BaseResponse>(null);

            _tvTaskList = tvTaskList;
            _tbSearchBox = tbSearchBox;

            var alertNodesRoot = new TreeNodeData();
            var treeNodeDataFromTreeViewRoot = _treeNodeToTreeNodeDataAdapter.AdaptTreeView(tvTaskList);
            var haveAlerts = _importTreeFromXmlService.LoadTreeNodesByCategory(treeNodeDataFromTreeViewRoot, alertNodesRoot, true);

            if (haveAlerts)
            {
                var form = new StartupAlertForm(_treeNodeDataToTreeNodeAdapter, alertNodesRoot);
                form.FormClosing += StartupAlertForm_FormClosing;
                form.ShowDialog();
                tvTaskList.Refresh();
            }

            return Task.FromResult(new BaseResponse());
        }

        private async void StartupAlertForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var form = sender as StartupAlertForm;
            if (form != null)
            {
                var selectedNode = form.SelectedItemOrCategory;
                var textToFind = selectedNode.Text;
                if (textToFind == "None")
                    return;

                _tbSearchBox.Text = textToFind;

                var request = new SearchBoxKeyUpRequest
                {
                    TreeView = _tvTaskList,
                    SearchBoxTextBox = _tbSearchBox,
                    KeyData = (int)Keys.Enter,
                };
                await _mediator.Send(request);

                ////if (tvTaskList.Nodes.Contains(selectedNode))
                ////    tvTaskList.SelectedNode = selectedNode;
                ////else
                ////    ;// search whole category (do nothing for now)
            }
        }
    }
}