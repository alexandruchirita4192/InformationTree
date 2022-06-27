using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormCompleteProgressValueChangedHandler : IRequestHandler<MainFormCompleteProgressValueChangedRequest, BaseResponse>
    {
        private readonly IMediator _mediator;
        private readonly ITreeNodeToTreeNodeDataAdapter _treeNodeToTreeNodeDataAdapter;

        public MainFormCompleteProgressValueChangedHandler(
            IMediator mediator,
            ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter
            )
        {
            _mediator = mediator;
            _treeNodeToTreeNodeDataAdapter = treeNodeToTreeNodeDataAdapter;
        }

        public async Task<BaseResponse> Handle(MainFormCompleteProgressValueChangedRequest request, CancellationToken cancellationToken)
        {
            if (request.PercentCompleteProgressBar is not ProgressBar pbPercentComplete)
                return null;
            if (request.CompleteProgressNumericUpDown is not NumericUpDown nudCompleteProgress)
                return null;
            if (request.SelectedNode is not TreeNode selectedNode)
                return null;

            pbPercentComplete.Maximum = 100;

            var progressValue = nudCompleteProgress.Value
                .ValidatePercentage();
            var isPercentCompletedChanged = pbPercentComplete.Value != (int)progressValue;
            if (isPercentCompletedChanged)
                pbPercentComplete.Value = (int)progressValue;

            var selectedNodeData = _treeNodeToTreeNodeDataAdapter.Adapt(selectedNode);

            var isSelectedNodeDataPercentChagnged = selectedNodeData.PercentCompleted != progressValue;
            if (isSelectedNodeDataPercentChagnged)
                selectedNodeData.PercentCompleted = progressValue;

            var percentCompletedValueHasChanged = isPercentCompletedChanged || isSelectedNodeDataPercentChagnged;
            if (percentCompletedValueHasChanged)
            {
                var setTreeStateRequest = new SetTreeStateRequest
                {
                    TreeUnchanged = false
                };
                await _mediator.Send(setTreeStateRequest, cancellationToken);
            }

            return new BaseResponse();
        }
    }
}