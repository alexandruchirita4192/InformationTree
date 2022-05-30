using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class TreeViewMouseMoveHandler : IRequestHandler<TreeViewMouseMoveRequest, BaseResponse>
    {
        private readonly ICachingService _cachingService;

        public TreeViewMouseMoveHandler(ICachingService cachingService)
        {
            _cachingService = cachingService;
        }

        public Task<BaseResponse> Handle(TreeViewMouseMoveRequest request, CancellationToken cancellationToken)
        {
            if (request.TreeView is not TreeView tvTaskList)
                return Task.FromResult<BaseResponse>(null);
            if (request.Tooltip is not ToolTip ttTaskList)
                return Task.FromResult<BaseResponse>(null);

            var _oldX = _cachingService.Get<int>(Constants.CacheKeys.TreeViewOldX);
            var _oldY = _cachingService.Get<int>(Constants.CacheKeys.TreeViewOldY);
            if (request.X == _oldX && request.Y == _oldY)
                return Task.FromResult<BaseResponse>(null);

            // Get the node at the current mouse pointer location.
            TreeNode theNode = tvTaskList.GetNodeAt(request.X, request.Y);

            // Set a ToolTip only if the mouse pointer is actually paused on a node.
            if (theNode != null)
            {
                // Change the ToolTip only if the pointer moved to a new node.
                if (theNode.ToolTipText != ttTaskList.GetToolTip(tvTaskList))
                {
                    ttTaskList.SetToolTip(tvTaskList, theNode.ToolTipText);
                }
            }
            else // Pointer is not over a node so clear the ToolTip.
            {
                ttTaskList.SetToolTip(tvTaskList, "");
            }

            _cachingService.Set(Constants.CacheKeys.TreeViewOldX, request.X);
            _cachingService.Set(Constants.CacheKeys.TreeViewOldY, request.Y);

            return Task.FromResult(new BaseResponse());
        }
    }
}