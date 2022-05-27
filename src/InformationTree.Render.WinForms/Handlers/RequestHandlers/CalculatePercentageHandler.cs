using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.TextProcessing;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class CalculatePercentageHandler : IRequestHandler<CalculatePercentageRequest, BaseResponse>
    {
        private readonly IMediator _mediator;

        public CalculatePercentageHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<BaseResponse> Handle(CalculatePercentageRequest request, CancellationToken cancellationToken)
        {
            if (request.SelectedNode is not TreeNode selectedNode)
                return null;

            if (request.Direction == CalculatePercentageDirection.FromLeafsToSelectedNode)
            {
                var percentage = (decimal)GetPercentageFromChildren(selectedNode);
                selectedNode.Text = TextProcessingHelper.UpdateTextAndProcentCompleted(selectedNode.Text, ref percentage, true);

                var setTreeStateRequest = new SetTreeStateRequest
                {
                    TreeUnchanged = false
                };
                return await _mediator.Send(setTreeStateRequest, cancellationToken);
            }
            else if (request.Direction == CalculatePercentageDirection.FromSelectedNodeToLeafs)
            {
                var percentage = 0.0M;
                TextProcessingHelper.GetTextAndProcentCompleted(selectedNode.Text, ref percentage, true);
                SetPercentageToChildren(selectedNode, (double)percentage);

                var setTreeStateRequest = new SetTreeStateRequest
                {
                    TreeUnchanged = false
                };
                return await _mediator.Send(setTreeStateRequest, cancellationToken);
            }
            return new BaseResponse();
        }

        private double GetPercentageFromChildren(TreeNode topNode)
        {
            if (topNode == null)
                throw new ArgumentNullException(nameof(topNode));

            var sum = 0.0;
            var nr = 0;
            foreach (TreeNode node in topNode.Nodes)
            {
                if (node != null && node.Nodes.Count > 0)
                {
                    var procentCompleted = (decimal)GetPercentageFromChildren(node);
                    node.Text = TextProcessingHelper.UpdateTextAndProcentCompleted(node.Text, ref procentCompleted, true);

                    sum += (double)procentCompleted;
                }
                else
                {
                    var value = 0.0M;
                    node.Text = TextProcessingHelper.GetTextAndProcentCompleted(node.Text, ref value, true);

                    sum += (double)value;
                }
                nr++;
            }

            return nr != 0 ? sum / nr : 0;
        }

        private void SetPercentageToChildren(TreeNode topNode, double percentage)
        {
            if (topNode == null)
                throw new ArgumentNullException(nameof(topNode));
            if (percentage < 0 || percentage > 100)
                throw new ArgumentOutOfRangeException(nameof(percentage));

            foreach (TreeNode node in topNode.Nodes)
            {
                if (node != null)
                {
                    var percentCompleted = (decimal)percentage;
                    node.Text = TextProcessingHelper.UpdateTextAndProcentCompleted(node.Text, ref percentCompleted, true);
                    if (node.Nodes.Count > 0)
                        SetPercentageToChildren(node, percentage);
                }
            }
        }
    }
}