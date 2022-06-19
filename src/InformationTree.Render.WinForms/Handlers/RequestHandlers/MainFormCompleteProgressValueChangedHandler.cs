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
            pbPercentComplete.Value = (int)nudCompleteProgress.Value;

            if (selectedNode != null)
            {
                _treeNodeToTreeNodeDataAdapter.Adapt(selectedNode)
                    .PercentCompleted = nudCompleteProgress.Value
                    .ValidatePercentage();

                // TODO: Find a proper way to check if progress value has changed and not because of selection change
                var valueHasChanged = false;
                if (valueHasChanged)
                {
                    var setTreeStateRequest = new SetTreeStateRequest
                    {
                        TreeUnchanged = false
                    };
                    await _mediator.Send(setTreeStateRequest, cancellationToken);
                }
            }

            return new BaseResponse();
        }
    }
}