using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormAddTaskClickHandler : IRequestHandler<MainFormAddTaskClickRequest, BaseResponse>
    {
        private readonly ITreeNodeToTreeNodeDataAdapter _treeNodeToTreeNodeDataAdapter;
        private readonly IMediator _mediator;

        public MainFormAddTaskClickHandler(
            ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter,
            IMediator mediator
            )
        {
            _treeNodeToTreeNodeDataAdapter = treeNodeToTreeNodeDataAdapter;
            _mediator = mediator;
        }
        
        public async Task<BaseResponse> Handle(MainFormAddTaskClickRequest request, CancellationToken cancellationToken)
        {
            if (request.AfterSelectRequest == null)
                return null;
            if (request.AfterSelectRequest.CompleteProgressNumericUpDown is not NumericUpDown nudCompleteProgress)
                return null;
            if (request.AfterSelectRequest.TaskNameTextBox is not TextBox tbTaskName)
                return null;
            if (request.AfterSelectRequest.UrgencyNumericUpDown is not NumericUpDown nudUrgency)
                return null;
            if (request.AfterSelectRequest.LinkTextBox is not TextBox tbLink)
                return null;
            if (request.AfterSelectRequest.TreeView is not TreeView tvTaskList)
                return null;
            if (request.AfterSelectRequest.CategoryTextBox is not TextBox tbCategory)
                return null;
            if (request.AfterSelectRequest.IsStartupAlertCheckBox is not CheckBox cbIsStartupAlert)
                return null;
            if (request.ShowUntilNumberNumericUpDown is not NumericUpDown nudShowUntilNumber)
                return null;
            if (request.ShowFromNumberNumericUpDown is not NumericUpDown nudShowFromNumber)
                return null;
            
            var taskPercentCompleted = request.AfterSelectRequest.TaskPercentCompleted;
            var taskName = tbTaskName.Text;
            var urgency = (int)nudUrgency.Value;
            var link = tbLink.Text;

            // update part
            var selectedNode = tvTaskList.SelectedNode;
            var selectedNodeLastChildren = selectedNode != null && selectedNode.Nodes.Count > 0 ? selectedNode.Nodes[selectedNode.Nodes.Count - 1] : null;
            if (selectedNode != null && selectedNode.Text.Equals(taskName /*StartsWith + " [" */))
            {
                if (selectedNode.Text != taskName)
                {
                    selectedNode.Text = taskName;
                    var tagData = _treeNodeToTreeNodeDataAdapter.Adapt(selectedNode);
                    tagData.LastChangeDate = DateTime.Now;

                    await _mediator.Send(request.AfterSelectRequest, cancellationToken);

                    var setTreeStateRequest = new SetTreeStateRequest
                    {
                        TreeUnchanged = false
                    };
                    await _mediator.Send(setTreeStateRequest, cancellationToken);
                }
            }
            else if (selectedNodeLastChildren != null && selectedNodeLastChildren.Text.Equals(taskName /* StartsWith + " [" */))
            {
                if (selectedNodeLastChildren.Text != taskName)
                {
                    selectedNodeLastChildren.Text = taskName;

                    var tagData = _treeNodeToTreeNodeDataAdapter.Adapt(selectedNodeLastChildren);
                    tagData.LastChangeDate = DateTime.Now;

                    await _mediator.Send(request.AfterSelectRequest, cancellationToken);

                    var setTreeStateRequest = new SetTreeStateRequest
                    {
                        TreeUnchanged = false
                    };
                    await _mediator.Send(setTreeStateRequest, cancellationToken);
                }
            }
            else //insert
            {
                var node = new TreeNode(taskName)
                {
                    Name = 0.ToString(),
                    ForeColor = Constants.Colors.DefaultForeGroundColor,
                    BackColor = Constants.Colors.DefaultBackGroundColor,
                    NodeFont = WinFormsConstants.FontDefaults.DefaultFont.Clone() as Font,
                    ToolTipText = taskName.GetToolTipText()
                };
                var treeNodeData = _treeNodeToTreeNodeDataAdapter.Adapt(node);
                treeNodeData.AddedNumber = tvTaskList.GetNodeCount(true) + 1;
                treeNodeData.Urgency = urgency;
                treeNodeData.Link = link;
                treeNodeData.IsStartupAlert = false;
                treeNodeData.PercentCompleted = taskPercentCompleted;

                node.Text = taskName;

                if (tvTaskList.SelectedNode == null)
                    tvTaskList.Nodes.Add(node);
                else
                {
                    tvTaskList.SelectedNode.Nodes.Add(node);
                    tvTaskList.SelectedNode.Expand();
                }

                await _mediator.Send(request.AfterSelectRequest, cancellationToken);

                // on control add TreeUnchanged is set false too (tvTaskList_ControlAdded)
            }

            var updateNodeCountRequest = new UpdateNodeCountRequest
            {
                TreeView = tvTaskList,
                ShowUntilNumberNumericUpDown = nudShowUntilNumber,
                ShowFromNumberNumericUpDown = nudShowFromNumber,
            };
            await _mediator.Send(updateNodeCountRequest);

            // TODO: Fix properly
            // workaround fix for some weirdly added spaces; even with this fix the spaces are not always removed, and update text click request usually fixed that
            if (tvTaskList.SelectedNode == null)
                return null;

            var updateTextClickRequest = new UpdateTextClickRequest
            {
                AfterSelectRequest = request.AfterSelectRequest
            };

            await _mediator.Send(updateTextClickRequest, cancellationToken);

            return new BaseResponse();
        }
    }
}