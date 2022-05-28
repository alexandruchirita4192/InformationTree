using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Render.WinForms.Extensions;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class UpdateTextClickHandler : IRequestHandler<UpdateTextClickRequest, BaseResponse>
    {
        private readonly IMediator _mediator;
        private readonly ITreeNodeDataCachingService _treeNodeDataCachingService;

        public UpdateTextClickHandler(IMediator mediator, ITreeNodeDataCachingService treeNodeDataCachingService)
        {
            _mediator = mediator;
            _treeNodeDataCachingService = treeNodeDataCachingService;
        }

        public async Task<BaseResponse> Handle(UpdateTextClickRequest request, CancellationToken cancellationToken)
        {
            var selectedNode = request.SelectedNode;
            if (selectedNode == null)
                return null;

            selectedNode.Urgency = request.Urgency;
            selectedNode.Link = request.Link;
            selectedNode.Category = request.Category;
            selectedNode.IsStartupAlert = request.IsStartupAlert;
            selectedNode.PercentCompleted = request.TaskPercentCompleted;
            selectedNode.LastChangeDate = DateTime.Now;
            selectedNode.Text = request.TaskName;

            await _mediator.Send(request.AfterSelectRequest, cancellationToken);

            var setTreeStateRequest = new SetTreeStateRequest
            {
                TreeUnchanged = false
            };
            await _mediator.Send(setTreeStateRequest, cancellationToken);

            if (request.TaskListTreeView is TreeView tvTaskList)
            {
                tvTaskList.InvokeWrapper(tvTaskList =>
                {
                    tvTaskList.SelectedNode.Copy(selectedNode.ToTreeNode(_treeNodeDataCachingService), _treeNodeDataCachingService);
                });
            }

            return new BaseResponse();
        }
    }
}