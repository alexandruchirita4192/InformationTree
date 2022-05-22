using System.ComponentModel;
using System.Threading.Tasks;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Services
{
    public class ImportExportTreeXmlService : IImportExportTreeXmlService
    {
        private readonly IImportTreeFromXmlService _importTreeFromXmlService;
        private readonly IExportTreeToXmlService _exportTreeToXmlService;
        private readonly IMediator _mediator;

        public ImportExportTreeXmlService(
            IImportTreeFromXmlService importTreeFromXmlService,
            IExportTreeToXmlService exportTreeToXmlService,
            IMediator mediator)
        {
            _importTreeFromXmlService = importTreeFromXmlService;
            _exportTreeToXmlService = exportTreeToXmlService;
            _mediator = mediator;
        }

        public (TreeNodeData rootNode, string fileName) SaveCurrentTreeAndLoadAnother(
            TreeNodeData currentRoot,
            Component controlToSetWaitCursor,
            Component treeView,
            Component showUntilNumberNumericUpDown,
            Component showFromNumberNumericUpDown,
            string fileName)
        {
            _exportTreeToXmlService.SaveTree(currentRoot, fileName);

            var setTreeStateRequest = new SetTreeStateRequest
            {
                File = new FileInfo { FileName = fileName }
            };
            Task.Run(async () =>
            {
                return await _mediator.Send(setTreeStateRequest);
            }).Wait();

            var returnData = _importTreeFromXmlService.LoadTree(fileName, controlToSetWaitCursor);

            var treeViewCollapseAndRefreshRequest = new TreeViewCollapseAndRefreshRequest
            {
                TreeView = treeView
            };
            var updateNodeCountRequest = new UpdateNodeCountRequest
            {
                TreeView = treeView,
                ShowUntilNumberNumericUpDown = showUntilNumberNumericUpDown,
                ShowFromNumberNumericUpDown = showFromNumberNumericUpDown,
            };
            Task.Run(async () =>
            {
                await _mediator.Send(treeViewCollapseAndRefreshRequest);
                return await _mediator.Send(updateNodeCountRequest);
            }).Wait();

            return returnData;
        }
    }
}