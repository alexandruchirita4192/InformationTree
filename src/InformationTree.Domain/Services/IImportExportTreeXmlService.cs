using System;
using System.ComponentModel;
using InformationTree.Domain.Entities;

namespace InformationTree.Domain.Services
{
    public interface IImportExportTreeXmlService
    {
        /// <summary>
        /// Save tree from <paramref name="currentRoot"/> to xml file <paramref name="fileName"/>,
        /// executes some functions and returns the loaded tree root node with loaded (selected) file name.
        /// </summary>
        /// <param name="treeView">Used for collapse and refresh tree view and update node count</param>
        /// <param name="showUntilNumberNumericUpDown">Used for updating node count to nudShowUntilNumber control</param>
        /// <param name="showFromNumberNumericUpDown">Used for updating node count to nudShowFromNumber control</param>
        public (TreeNodeData rootNode, string fileName) SaveCurrentTreeAndLoadAnother(
            TreeNodeData currentRoot,
            Component controlToSetWaitCursor,
            Component treeView,
            Component showUntilNumberNumericUpDown,
            Component showFromNumberNumericUpDown,
            string fileName);
    }
}