using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormsGame.Forms
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
            foreach(var node in selectedNode.Nodes)
            {
                clbTasks.Items.Add(node, firstItem);
                firstItem = false;
            }
        }
    }
}
