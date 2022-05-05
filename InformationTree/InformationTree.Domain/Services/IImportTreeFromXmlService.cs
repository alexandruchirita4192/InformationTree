using System;
using System.Collections.Generic;
using InformationTree.Domain.Entities;

namespace InformationTree.Domain.Services
{
    public interface IImportTreeFromXmlService
    {
        /// <summary>
        /// Load tree from xml file with name <paramref name="fileName"/> and execute some actions
        /// </summary>
        public TreeNodeData LoadXML(string fileName, Action beforeLoad, Action afterLoad);

        /// <summary>
        /// Load tree with a file dialog selecting which file should be loaded, defaulting <paramref name="fileName"/> if it exists.
        /// </summary>
        /// <returns>The root node and file name selected</returns>
        public (TreeNodeData rootNode, string fileName) LoadTree(string fileName, Action beforeLoad, Action afterLoad);

        bool LoadTreeNodesByCategory(TreeNodeData from, TreeNodeData to, bool includeOnlyWithStartupAlert, Dictionary<string, TreeNodeData> categories = null);
    }
}