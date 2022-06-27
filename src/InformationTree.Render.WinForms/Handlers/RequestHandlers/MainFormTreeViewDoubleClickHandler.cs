using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Render.WinForms.Extensions;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormTreeViewDoubleClickHandler : IRequestHandler<MainFormTreeViewDoubleClickRequest, BaseResponse>
    {
        private readonly IMediator _mediator;
        private readonly IImportExportTreeXmlService _importExportTreeXmlService;
        private readonly ITreeNodeToTreeNodeDataAdapter _treeNodeToTreeNodeDataAdapter;

        public MainFormTreeViewDoubleClickHandler(
            IMediator mediator,
            IImportExportTreeXmlService importExportTreeXmlService,
            ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter
            )
        {
            _mediator = mediator;
            _importExportTreeXmlService = importExportTreeXmlService;
            _treeNodeToTreeNodeDataAdapter = treeNodeToTreeNodeDataAdapter;
        }

        public async Task<BaseResponse> Handle(MainFormTreeViewDoubleClickRequest request, CancellationToken cancellationToken)
        {
            if (request.TreeView is not TreeView tvTaskList)
                return null;
            if (tvTaskList.SelectedNode == null)
                return null;
            if (request.ShowFromNumberNumericUpDown is not NumericUpDown nudShowFromNumber)
                return null;
            if (request.ShowUntilNumberNumericUpDown is not NumericUpDown nudShowUntilNumber)
                return null;

            if (tvTaskList.SelectedNode != null)
            {
                var treeNodeData = _treeNodeToTreeNodeDataAdapter.Adapt(tvTaskList.SelectedNode);
                if (treeNodeData != null && treeNodeData.Link.IsNotEmpty())
                {
                    (_, var fileName) = _importExportTreeXmlService.SaveCurrentTreeAndLoadAnother(
                        _treeNodeToTreeNodeDataAdapter.AdaptTreeView(tvTaskList),
                        request.ControlToSetWaitCursor,
                        tvTaskList,
                        nudShowUntilNumber,
                        nudShowFromNumber,
                        treeNodeData.Link);

                    var setTreeStateRequest = new SetTreeStateRequest
                    {
                        FileInformation = new FileInformation { FileName = fileName }
                    };
                    await _mediator.Send(setTreeStateRequest, cancellationToken);
                }
            }
            else
            {
                var figureLines = tvTaskList.GenerateStringGraphicsLinesFromTree();

                var showCanvasPopUpRequest = new ShowCanvasPopUpRequest
                {
                    FigureLines = figureLines
                };
                await _mediator.Send(showCanvasPopUpRequest, cancellationToken);
            }

            return new BaseResponse();
        }
    }
}