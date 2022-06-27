using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormMoveTaskHandler : IRequestHandler<MainFormMoveTaskRequest, BaseResponse>
    {
        private readonly IPopUpService _popUpService;
        private readonly IMediator _mediator;

        public MainFormMoveTaskHandler(
            IPopUpService popUpService,
            IMediator mediator
            )
        {
            _popUpService = popUpService;
            _mediator = mediator;
        }

        public async Task<BaseResponse> Handle(MainFormMoveTaskRequest request, CancellationToken cancellationToken)
        {
            if (request.TreeView is not TreeView tvTaskList)
                return null;

            var directionText = request.MoveDirection == MoveTaskDirection.Up ? "up" : "down";
            var selectedNode = tvTaskList.SelectedNode;
            
            if (selectedNode != null
                && selectedNode.Parent != null
                && selectedNode.Parent.Nodes != null)
            {
                var parent = selectedNode.Parent;
                var selectedIndex = parent.Nodes.IndexOf(selectedNode);
                var count = parent.Nodes.Count;
                
                if (CanTaskMove(selectedIndex, count, request.MoveDirection))
                {
                    var nextIndex = GetNextIndex(selectedIndex, request.MoveDirection);
                    parent.Nodes.RemoveAt(selectedIndex);
                    parent.Nodes.Insert(nextIndex, selectedNode);
                    tvTaskList.SelectedNode = selectedNode;
                }
                else
                    _popUpService.ShowMessage($"Cannot move task {directionText}! Current index is {selectedIndex}.");
            }
            else if (selectedNode != null
                && selectedNode.Parent == null)
            {
                var selectedIndex = tvTaskList.Nodes.IndexOf(selectedNode);
                var count = tvTaskList.Nodes.Count;
                
                if (CanTaskMove(selectedIndex, count, request.MoveDirection))
                {
                    var nextIndex = GetNextIndex(selectedIndex, request.MoveDirection);
                    tvTaskList.Nodes.RemoveAt(selectedIndex);
                    tvTaskList.Nodes.Insert(nextIndex, selectedNode);
                    tvTaskList.SelectedNode = selectedNode;
                }
                else
                    _popUpService.ShowMessage($"Cannot move task {directionText}! Current index is {selectedIndex}.");
            }

            var setTreeStateRequest = new SetTreeStateRequest
            {
                TreeUnchanged = false
            };
            await _mediator.Send(setTreeStateRequest, cancellationToken);
            return new BaseResponse();
        }

        private static bool CanTaskMove(
            int selectedIndex,
            int count,
            MoveTaskDirection direction)
        {
            return direction == MoveTaskDirection.Up
                ? selectedIndex > 0 // Moving the task up is possible if the selected task is not the first one, with index 1
                : selectedIndex < count; // Moving the task down is possible if the selected task is not the last one, with index [count]
        }

        private static int GetNextIndex(
            int selectedIndex,
            MoveTaskDirection direction)
        {
            return direction == MoveTaskDirection.Up
                ? selectedIndex - 1
                : selectedIndex + 1;
        }
    }
}