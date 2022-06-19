using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormGoToDefaultTreeClickHandler : IRequestHandler<MainFormGoToDefaultTreeClickRequest, BaseResponse>
    {
        private readonly IMediator _mediator;
        private readonly IImportExportTreeXmlService _importExportTreeXmlService;
        private readonly ITreeNodeToTreeNodeDataAdapter _treeNodeToTreeNodeDataAdapter;

        public MainFormGoToDefaultTreeClickHandler(
            IMediator mediator,
            IImportExportTreeXmlService importExportTreeXmlService,
            ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter
            )
        {
            _mediator = mediator;
            _importExportTreeXmlService = importExportTreeXmlService;
            _treeNodeToTreeNodeDataAdapter = treeNodeToTreeNodeDataAdapter;
        }

        public async Task<BaseResponse> Handle(MainFormGoToDefaultTreeClickRequest request, CancellationToken cancellationToken)
        {
            if (request.TreeView is not TreeView tvTaskList)
                return null;
            if (request.ShowUntilNumberNumericUpDown is not NumericUpDown nudShowUntilNumber)
                return null;
            if (request.ShowFromNumberNumericUpDown is not NumericUpDown nudShowFromNumber)
                return null;

            (_, var fileName) = _importExportTreeXmlService.SaveCurrentTreeAndLoadAnother(
                _treeNodeToTreeNodeDataAdapter.AdaptTreeView(tvTaskList),
                request.ControlToSetWaitCursor,
                tvTaskList,
                nudShowUntilNumber,
                nudShowFromNumber,
                null);

            var setTreeStateRequest = new SetTreeStateRequest
            {
                FileInformation = new FileInformation { FileName = fileName }
            };
            return await _mediator.Send(setTreeStateRequest, cancellationToken);
        }
    }
}