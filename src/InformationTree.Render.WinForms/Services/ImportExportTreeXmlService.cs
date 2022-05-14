using System;
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

        // TODO: Too many actions!! Maybe change with some real events with an event dispatcher?????? Does it break everything? What is the best practice??
        public (TreeNodeData rootNode, string fileName) SaveCurrentTreeAndLoadAnother(
            Action<string> afterSaveDoWithFileName,
            TreeNodeData currentRoot,
            string fileName,
            Action beforeLoadInside,
            Action afterLoadInside,
            Action afterLoad)
        {
            _exportTreeToXmlService.SaveTree(currentRoot, fileName);
            
            afterSaveDoWithFileName?.Invoke(fileName);

            var returnData = _importTreeFromXmlService.LoadTree(fileName, beforeLoadInside, afterLoadInside);

            afterLoad?.Invoke();

            return returnData;
        }
    }
}