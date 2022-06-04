using System;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Services;

namespace InformationTree.Forms
{
    public partial class StartupAlertForm : Form
    {
        public TreeNode SelectedItemOrCategory
        {
            get
            {
                return tvAlertCategoryAndTasks != null ? tvAlertCategoryAndTasks.SelectedNode : null;
            }
        }

        public StartupAlertForm(ITreeNodeDataToTreeNodeAdapter treeNodeDataToTreeNodeAdapter, TreeNodeData treeNodeData = null)
        {
            InitializeComponent();

            if (treeNodeData != null && tvAlertCategoryAndTasks?.Nodes != null)
            {
                tvAlertCategoryAndTasks.Nodes.Add(new TreeNode("None"));
                foreach (TreeNodeData childTreeNodeData in treeNodeData.Children)
                    if (treeNodeDataToTreeNodeAdapter.Adapt(childTreeNodeData, false) is TreeNode childTreeNode)
                        tvAlertCategoryAndTasks.Nodes.Add(childTreeNode);
            }

            StartPosition = FormStartPosition.CenterScreen;
        }

        // TODO: Handler for FormCloseRequest
        private void btnSelectTaskOrCategory_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}