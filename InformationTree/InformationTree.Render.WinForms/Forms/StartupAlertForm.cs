using System;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Services;
using InformationTree.Render.WinForms.Extensions;

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

        public StartupAlertForm(ITreeNodeDataCachingService treeNodeDataCachingService, TreeNodeData treeNodeData = null)
        {
            InitializeComponent();

            if (treeNodeData != null && tvAlertCategoryAndTasks?.Nodes != null)
            {
                tvAlertCategoryAndTasks.Nodes.Add(new TreeNode("None"));
                foreach (TreeNodeData child in treeNodeData.Children)
                    tvAlertCategoryAndTasks.Nodes.Add(child.ToTreeNode(treeNodeDataCachingService, false));
            }

            StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnSelectTaskOrCategory_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}