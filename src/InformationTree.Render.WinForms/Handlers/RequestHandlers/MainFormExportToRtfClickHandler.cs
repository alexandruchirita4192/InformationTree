using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormExportToRtfClickHandler : IRequestHandler<MainFormExportToRtfClickRequest, BaseResponse>
    {
        private readonly IExportNodeToRtfService _exportNodeToRtfService;

        public MainFormExportToRtfClickHandler(
            IExportNodeToRtfService exportNodeToRtfService
            )
        {
            _exportNodeToRtfService = exportNodeToRtfService;
        }

        public Task<BaseResponse> Handle(MainFormExportToRtfClickRequest request, CancellationToken cancellationToken)
        {
            if (request.SelectedNode is not TreeNode selectedNode)
                return Task.FromResult<BaseResponse>(null);

            var treeNodeData = ParseTreeRecursively(selectedNode);
            var exportedRtf = _exportNodeToRtfService.GetRtfExport(treeNodeData);

            Clipboard.SetText(exportedRtf, TextDataFormat.Rtf);

            return Task.FromResult(new BaseResponse());
        }

        private TreeNodeData ParseTreeRecursively(TreeNode currentNode)
        {
            if (currentNode == null)
                return new TreeNodeData();

            var treeNodeData = new TreeNodeData { Text = currentNode.Text };

            if (currentNode.Nodes != null && currentNode.Nodes.Count > 0)
            {
                foreach (TreeNode child in currentNode.Nodes)
                {
                    var childTreeNodeData = ParseTreeRecursively(child);
                    treeNodeData.Children.Add(childTreeNodeData);
                }
            }

            return treeNodeData;
        }
    }
}