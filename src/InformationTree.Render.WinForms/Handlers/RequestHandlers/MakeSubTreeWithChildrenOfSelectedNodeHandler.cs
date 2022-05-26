using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Render.WinForms.Extensions;
using InformationTree.TextProcessing;
using InformationTree.Tree;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MakeSubTreeWithChildrenOfSelectedNodeHandler : IRequestHandler<MakeSubTreeWithChildrenOfSelectedNodeRequest, BaseResponse>
    {
        private readonly IMediator _mediator;
        private readonly ITreeNodeDataCachingService _treeNodeDataCachingService;
        private readonly IExportTreeToXmlService _exportTreeToXmlService;

        public MakeSubTreeWithChildrenOfSelectedNodeHandler(
            IMediator mediator,
            ITreeNodeDataCachingService treeNodeDataCachingService,
            IExportTreeToXmlService exportTreeToXmlService)
        {
            _mediator = mediator;
            _treeNodeDataCachingService = treeNodeDataCachingService;
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

                var tagData = node.ToTreeNodeData(_treeNodeDataCachingService);
                tagData.Link = tbLink.Text;

                var percentCompleted = 0M;
                if (tagData.Link.IsEmpty() || !tagData.Link.EndsWith(".xml") || tagData.Link.Contains(" "))
                    tagData.Link = TextProcessingHelper.GetTextAndProcentCompleted(node.Text, ref percentCompleted, true).Replace(" ", "_") + ".xml";

                tbLink.Text = tagData.Link;

                var treeView = new TreeView();
                if (useSelectedNode)
                {
                    var parentNode = new TreeNode();
                    TreeNodeHelper.CopyNode(parentNode, node, _treeNodeDataCachingService);
                    treeView.Nodes.Add(parentNode);
                    tagData.Data = null;
                }
                else
                {
                    TreeNodeHelper.CopyNodes(treeView.Nodes, node.Nodes, _treeNodeDataCachingService);
                }

                if (await _mediator.Send(new GetTreeStateRequest(), cancellationToken) is not GetTreeStateResponse getTreeStateResponse)
                    return null;

                var auxFileName = getTreeStateResponse.FileName;

                var setTreeStateFileInfoRequest = new SetTreeStateRequest
                {
                    FileInformation = new FileInformation { FileName = tagData.Link }
                };
                await _mediator.Send(setTreeStateFileInfoRequest, cancellationToken);

                if (await _mediator.Send(new GetTreeStateRequest(), cancellationToken) is not GetTreeStateResponse getTreeStateResponseUpdated)
                    return null;

                var root = treeView.ToTreeNodeData(_treeNodeDataCachingService);
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