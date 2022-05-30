using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Forms;
using InformationTree.Render.WinForms.Extensions;
using InformationTree.TextProcessing;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class ShowStartupAlertFormHandler : IRequestHandler<ShowStartupAlertFormRequest, BaseResponse>
    {
        private readonly IImportTreeFromXmlService _importTreeFromXmlService;
        private readonly IMediator _mediator;
        private readonly ITreeNodeDataCachingService _treeNodeDataCachingService;
        
        private TreeView _tvTaskList;
        private TextBox _tbSearchBox;

        public ShowStartupAlertFormHandler(
            IImportTreeFromXmlService importTreeFromXmlService,
            IMediator mediator,
            ITreeNodeDataCachingService treeNodeDataCachingService)
        {
            _importTreeFromXmlService = importTreeFromXmlService;
            _mediator = mediator;
            _treeNodeDataCachingService = treeNodeDataCachingService;
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
            var haveAlerts = _importTreeFromXmlService.LoadTreeNodesByCategory(tvTaskList.ToTreeNodeData(_treeNodeDataCachingService), alertNodesRoot, true);

            if (haveAlerts)
            {
                var form = new StartupAlertForm(_treeNodeDataCachingService, alertNodesRoot);
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

                var percent = 0m;
                _tbSearchBox.Text = TextProcessingHelper.GetTextAndProcentCompleted(textToFind, ref percent, true);

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