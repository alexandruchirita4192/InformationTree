using System;
using System.Collections.Generic;
using System.ComponentModel;
using InformationTree.Domain.Entities;

namespace InformationTree.Domain.Services
{
    public interface IImportTreeFromXmlService
    {
        /// <summary>
        /// Load tree from xml file with name <paramref name="fileName"/>
        /// </summary>
        public TreeNodeData LoadXML(string fileName, Component controlToSetWaitCursor);

        /// <summary>
        /// Load tree with a file dialog selecting which file should be loaded, defaulting <paramref name="fileName"/> if it exists.
        /// </summary>
        /// <returns>The root node and file name selected</returns>
        public (TreeNodeData rootNode, string fileName) LoadTree(string fileName, Component controlToSetWaitCursor);

        bool LoadTreeNodesByCategory(TreeNodeData from, TreeNodeData to, bool includeOnlyWithStartupAlert, Dictionary<string, TreeNodeData> categories = null);
    }
}