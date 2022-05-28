using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class TreeViewNoTaskHandler : IRequestHandler<TreeViewNoTaskRequest, BaseResponse>
    {
        private readonly IMediator _mediator;

        public TreeViewNoTaskHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<BaseResponse> Handle(TreeViewNoTaskRequest request, CancellationToken cancellationToken)
        {
            if (request.TreeView is not TreeView tvTaskList)
                return null;
            if (request.AfterSelectRequest == null)
                return null;

            tvTaskList.SelectedNode = null;

            // TODO: After checking thoughly that AfterSelectRequest does all necessary things, remove zombie code
            //tvTaskList_AfterSelect(
            //    sender,
            //    new TreeViewEventArgs(
            //        new TreeNode(null)
            //        {
            //            ToolTipText = null,
            //            Tag = new TreeNodeData(null)
            //        })
            //);
            //gbTimeSpent.Enabled = false;
            //tbAddedDate.Text = "-";
            //tbAddedNumber.Text = "-";
            //tbTaskName.Text = string.Empty;
            //nudHours.Value = 0;
            //nudMinutes.Value = 0;
            //nudSeconds.Value = 0;
            //nudMilliseconds.Value = 0;
            //nudUrgency.Value = 0;

            return await _mediator.Send(request.AfterSelectRequest, cancellationToken);
        }
    }
}