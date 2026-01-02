using System;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Services;
using MediatR;
using System.ComponentModel;

namespace InformationTree.Forms
{
    public partial class StartupAlertForm : Form
    {
        private readonly ITreeNodeDataToTreeNodeAdapter _treeNodeDataToTreeNodeAdapter;
        private readonly IMediator _mediator;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TreeNode SelectedItemOrCategory
        {
            get
            {
                return tvAlertCategoryAndTasks != null ? tvAlertCategoryAndTasks.SelectedNode : null;
            }
        }

        public StartupAlertForm()
        {
            InitializeComponent();
        }

        public StartupAlertForm(
            ITreeNodeDataToTreeNodeAdapter treeNodeDataToTreeNodeAdapter,
            IMediator mediator,
            TreeNodeData treeNodeData = null)
            : this()
        {
            _treeNodeDataToTreeNodeAdapter = treeNodeDataToTreeNodeAdapter;
            _mediator = mediator;
            
            if (treeNodeData != null && tvAlertCategoryAndTasks?.Nodes != null)
            {
                tvAlertCategoryAndTasks.Nodes.Add(new TreeNode("None"));
                foreach (TreeNodeData childTreeNodeData in treeNodeData.Children)
                    if (_treeNodeDataToTreeNodeAdapter.Adapt(childTreeNodeData, false) is TreeNode childTreeNode)
                        tvAlertCategoryAndTasks.Nodes.Add(childTreeNode);
            }

            StartPosition = FormStartPosition.CenterScreen;
        }

        private async void btnSelectTaskOrCategory_Click(object sender, EventArgs e)
        {
            var request = new FormCloseRequest
            {
                Form = this
            };
            await _mediator.Send(request);
        }
    }
}