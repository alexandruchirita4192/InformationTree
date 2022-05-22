using System;
using System.ComponentModel;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Services;

namespace InformationTree.Render.WinForms.Services
{
    public class ImportExportTreeXmlService : IImportExportTreeXmlService
    {
        private readonly IImportTreeFromXmlService _importTreeFromXmlService;
        private readonly IExportTreeToXmlService _exportTreeToXmlService;

        public ImportExportTreeXmlService(IImportTreeFromXmlService importTreeFromXmlService, IExportTreeToXmlService exportTreeToXmlService)
        {
            _importTreeFromXmlService = importTreeFromXmlService;
            _exportTreeToXmlService = exportTreeToXmlService;
        }

        // TODO: Fix with MediatR handlers because the actions were always the same (code is also duplicated regarding the called actions, they only need to have the required control access to do the stuff)
        // TODO: Too many actions!! Maybe change with some real events with an event dispatcher?????? Does it break everything? What is the best practice??
        public (TreeNodeData rootNode, string fileName) SaveCurrentTreeAndLoadAnother(
            Action<string> afterSaveDoWithFileName,
            TreeNodeData currentRoot,
            Component controlToSetWaitCursor,
            string fileName,
            Action afterLoad)
        {
            _exportTreeToXmlService.SaveTree(currentRoot, fileName);
            
            afterSaveDoWithFileName?.Invoke(fileName);

            var returnData = _importTreeFromXmlService.LoadTree(fileName, controlToSetWaitCursor);

            afterLoad?.Invoke();

            return returnData;
        }
    }
}