using System;
using System.Windows.Forms;

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

        public StartupAlertForm(TreeNodeCollection treeNodes = null)
        {
            InitializeComponent();

            if (treeNodes != null && tvAlertCategoryAndTasks != null && tvAlertCategoryAndTasks.Nodes != null)
            {
                tvAlertCategoryAndTasks.Nodes.Add(new TreeNode("None"));
                foreach (TreeNode node in treeNodes)
                    tvAlertCategoryAndTasks.Nodes.Add(node);
            }

            StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnSelectTaskOrCategory_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void StartupAlertForm_KeyUp(object sender, KeyEventArgs e)
        {
        }
    }
}