using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormStartCountingClick : IRequestHandler<MainFormStartCountingClickRequest, BaseResponse>
    {
        public Task<BaseResponse> Handle(MainFormStartCountingClickRequest request, CancellationToken cancellationToken)
        {
            if (request.SelectedNode is not TreeNode selectedNode)
                return Task.FromResult<BaseResponse>(null);
            if (request.TaskGroupBox is not GroupBox gbTask)
                return Task.FromResult<BaseResponse>(null);
            if (request.TaskListGroupBox is not GroupBox gbTaskList)
                return Task.FromResult<BaseResponse>(null);
            if (request.Timer is not Stopwatch timer)
                return Task.FromResult<BaseResponse>(null);

            timer.Start();
            gbTask.Enabled = false;
            gbTaskList.Enabled = false;

            return Task.FromResult(new BaseResponse());
        }
    }
}