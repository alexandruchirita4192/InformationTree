using System;
using InformationTree.Domain.Entities;

namespace InformationTree.Domain.Services
{
    public interface IImportExportTreeXmlService
    {
        /// <summary>
        /// Save tree from <paramref name="currentRoot"/> to xml file <paramref name="fileName"/>,
        /// executes some functions and returns the loaded tree root node with loaded (selected) file name.
        /// </summary>
        public (TreeNodeData rootNode, string fileName) SaveCurrentTreeAndLoadAnother(
            Action<string> afterSaveDoWithFileName,
            TreeNodeData currentRoot,
            string fileName,
            Action beforeLoadInside,
            Action afterLoadInside,
            Action afterLoad);
    }
}