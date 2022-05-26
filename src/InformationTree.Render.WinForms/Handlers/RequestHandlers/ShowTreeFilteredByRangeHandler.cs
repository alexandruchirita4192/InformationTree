using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Tree;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class ShowTreeFilteredByRangeHandler : IRequestHandler<ShowTreeFilteredByRangeRequest, BaseResponse>
    {
        private const string NodesListKey = "Nodes";

        private readonly IMediator _mediator;
        private readonly ITreeNodeDataCachingService _treeNodeDataCachingService;
        private readonly IListCachingService _listCachingService;

        public ShowTreeFilteredByRangeHandler(
            IMediator mediator,
            ITreeNodeDataCachingService treeNodeDataCachingService,
            IListCachingService listCachingService)
        {
            _mediator = mediator;
            _treeNodeDataCachingService = treeNodeDataCachingService;
            _listCachingService = listCachingService;
        }

        public async Task<BaseResponse> Handle(ShowTreeFilteredByRangeRequest request, CancellationToken cancellationToken)
        {
            if (request.TreeView is not TreeView tvTaskList)
                return null;
            if (request.ShowAllButton is not Button btnShowAll)
                return null;
            if (request.TaskGroupBox is not GroupBox gbTask)
                return null;
            if (request.StyleChangeGroupBox is not GroupBox gbStyleChange)
                return null;
            if (request.TimeSpentGroupBox is not GroupBox gbTimeSpent)
                return null;
            if (await _mediator.Send(new GetTreeStateRequest(), cancellationToken) is not GetTreeStateResponse getTreeStateResponse)
                return null;
            if (IsProperState(request.FilterType, getTreeStateResponse.ReadOnlyState) == false)
                return null;

            await ShowTasksBasedOn(
                tvTaskList,
                request.Max,
                request.Min,
                request.FilterType,
                cancellationToken);
            SetComponentsByFilterType(request.FilterType, btnShowAll, gbTask, gbStyleChange, gbTimeSpent);
            return new BaseResponse();
        }

        private static bool IsProperState(CopyNodeFilterType filterType, bool readOnlyState)
        {
            return (filterType == CopyNodeFilterType.NoFilter && readOnlyState == true)
                || (filterType == CopyNodeFilterType.FilterByAddedNumber && readOnlyState == false)
                || (filterType == CopyNodeFilterType.FilterByUrgency && readOnlyState == false);
        }

        private static void SetComponentsByFilterType(CopyNodeFilterType filterType, Button btnShowAll, GroupBox gbTask, GroupBox gbStyleChange, GroupBox gbTimeSpent)
        {
            var isShowAllEnabled = filterType != CopyNodeFilterType.NoFilter;
            var areOthersEnabled = filterType == CopyNodeFilterType.NoFilter;

            btnShowAll.Enabled = isShowAllEnabled;
            gbTask.Enabled = areOthersEnabled;
            gbStyleChange.Enabled = areOthersEnabled;
            gbTimeSpent.Enabled = areOthersEnabled;
        }

        private async Task ShowTasksBasedOn(
            TreeView tvTaskList,
            decimal filterLowerThan,
            decimal filterHigherThan,
            CopyNodeFilterType filterType,
            CancellationToken cancellationToken)
        {
            if (_listCachingService.Get(NodesListKey) is not TreeNodeCollection nodes)
            {
                nodes = new TreeNode().Nodes;
                _listCachingService.Set(NodesListKey, nodes);
            }

            nodes.Clear();
            var isFiltered = filterType != CopyNodeFilterType.NoFilter;
            if (isFiltered)
            {
                // Copy tree to cache to save current tree state
                TreeNodeHelper.CopyNodes(nodes, tvTaskList.Nodes, _treeNodeDataCachingService, null, null, CopyNodeFilterType.NoFilter);
                _listCachingService.Set(NodesListKey, nodes);

                // Update tree view with filtered nodes
                tvTaskList.Nodes.Clear();
                TreeNodeHelper.CopyNodes(tvTaskList.Nodes, nodes, _treeNodeDataCachingService, (int)filterHigherThan, (int)filterLowerThan, filterType);
            }
            else
            {
                // Copy tree back from cache to restore current tree state
                TreeNodeHelper.CopyNodes(tvTaskList.Nodes, nodes, _treeNodeDataCachingService, null, null, CopyNodeFilterType.NoFilter);
            }

            // Update read only state that keeps track of whether the tree is filtered or not
            var setTreeStateRequest = new SetTreeStateRequest
            {
                ReadOnlyState = isFiltered
            };
            await _mediator.Send(setTreeStateRequest, cancellationToken);
        }
    }
}