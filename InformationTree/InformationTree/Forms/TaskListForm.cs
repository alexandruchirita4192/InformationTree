using System.Windows.Forms;

namespace InformationTree.Forms
{
    public partial class TaskListForm : Form
    {
        public TaskListForm()
        {
            InitializeComponent();
        }

        public TaskListForm(TreeNode selectedNode)
            : this()
        {
            var firstItem = true;
            foreach (var node in selectedNode.Nodes)
            {
                clbTasks.Items.Add(node, firstItem);
                firstItem = false;
            }
        }
    }
}