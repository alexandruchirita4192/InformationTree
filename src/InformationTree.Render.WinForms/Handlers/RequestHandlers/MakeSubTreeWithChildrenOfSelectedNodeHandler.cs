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
    public class MakeSubTreeWithChildrenOfSelectedNodeHandler : IRequestHandler<MakeSubTreeWithChildrenOfSelectedNodeRequest, BaseResponse>
    {
        private readonly IMediator _mediator;
        private readonly ITreeNodeDataCachingService _treeNodeDataCachingService;
        private readonly ITreeNodeToTreeNodeDataAdapter _treeNodeToTreeNodeDataAdapter;
        private readonly IExportTreeToXmlService _exportTreeToXmlService;

        public MakeSubTreeWithChildrenOfSelectedNodeHandler(
            IMediator mediator,
            ITreeNodeDataCachingService treeNodeDataCachingService,
            ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter,
            IExportTreeToXmlService exportTreeToXmlService)
        {
            _mediator = mediator;
            _treeNodeDataCachingService = treeNodeDataCachingService;
            _treeNodeToTreeNodeDataAdapter = treeNodeToTreeNodeDataAdapter;
            _exportTreeToXmlService = exportTreeToXmlService;
        }

        public async Task<BaseResponse> Handle(MakeSubTreeWithChildrenOfSelectedNodeRequest request, CancellationToken cancellationToken)
        {
            if (request.SelectedTreeNode is not TreeNode selectedNode)
                return null;
            if (request.LinkTextBox is not TextBox tbLink)
                return null;

            var useSelectedNode = request.UseSelectedNode;

            var node = selectedNode;
            if (node != null)
            {
                if (node.Nodes.Count == 0 && !useSelectedNode)
                    return null;

                var treeNodeData = _treeNodeToTreeNodeDataAdapter.Adapt(node);
                treeNodeData.Link = tbLink.Text;

                if (treeNodeData.Link.IsEmpty() || !treeNodeData.Link.EndsWith(".xml") || treeNodeData.Link.Contains(" "))
                    treeNodeData.Link = node.Text.Replace(" ", "_") + ".xml";

                tbLink.Text = treeNodeData.Link;

                var treeView = new TreeView();
                if (useSelectedNode)
                {
                    var parentNode = new TreeNode();
                    parentNode.Copy(node, _treeNodeToTreeNodeDataAdapter);
                    treeView.Nodes.Add(parentNode);
                    treeNodeData.Data = null;
                }
                else
                {
                    treeView.Nodes.Copy(node.Nodes, _treeNodeToTreeNodeDataAdapter);
                }

                if (await _mediator.Send(new GetTreeStateRequest(), cancellationToken) is not GetTreeStateResponse getTreeStateResponse)
                    return null;

                var auxFileName = getTreeStateResponse.FileName;

                var setTreeStateFileInfoRequest = new SetTreeStateRequest
                {
                    FileInformation = new FileInformation { FileName = treeNodeData.Link }
                };
                await _mediator.Send(setTreeStateFileInfoRequest, cancellationToken);

                if (await _mediator.Send(new GetTreeStateRequest(), cancellationToken) is not GetTreeStateResponse getTreeStateResponseUpdated)
                    return null;

                var root = _treeNodeToTreeNodeDataAdapter.AdaptTreeView(treeView);
                _exportTreeToXmlService.SaveTree(root, getTreeStateResponseUpdated.FileName);

                setTreeStateFileInfoRequest = new SetTreeStateRequest
                {
                    FileInformation = new FileInformation { FileName = auxFileName }
                };
                await _mediator.Send(setTreeStateFileInfoRequest, cancellationToken);

                node.Nodes.Clear();
            }

            var setTreeStateRequest = new SetTreeStateRequest
            {
                TreeUnchanged = false
            };
            return await _mediator.Send(setTreeStateRequest, cancellationToken);
        }
    }
}