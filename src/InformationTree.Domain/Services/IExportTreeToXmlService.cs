using InformationTree.Domain.Entities;

namespace InformationTree.Domain.Services
{
    public interface IExportTreeToXmlService
    {
        /// <summary>
        /// Save tree from <paramref name="treeNodeData"/> to xml file <paramref name="fileName"/>
        /// </summary>
        void SaveTree(TreeNodeData treeNodeData, string fileName);
    }
}