using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Render.WinForms.Extensions;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    internal class SearchBoxKeyUpHandler : IRequestHandler<SearchBoxKeyUpRequest, BaseResponse>
    {
        private readonly ICachingService _cachingService;
        private readonly ITreeNodeDataCachingService _treeNodeDataCachingService;

        public SearchBoxKeyUpHandler(
            ICachingService cachingService,
            ITreeNodeDataCachingService treeNodeDataCachingService)
        {
            _cachingService = cachingService;
            _treeNodeDataCachingService = treeNodeDataCachingService;
        }

        public Task<BaseResponse> Handle(SearchBoxKeyUpRequest request, CancellationToken cancellationToken)
        {
            if (request.SearchBoxTextBox is not TextBox tbSearchBox)
                return Task.FromResult<BaseResponse>(null);
            if (request.TreeView is not TreeView tvTaskList)
                return Task.FromResult<BaseResponse>(null);

            _cachingService.Set(Constants.CacheKeys.IsControlKeyPressed, false);
            if (request.KeyData == (int)Keys.Enter)
            {
                var searchText = tbSearchBox.Text;

                tvTaskList.Nodes.ClearStyleAdded();

                if (searchText.Length < 3)
                    return Task.FromResult<BaseResponse>(null);

                if (searchText.IsNotEmpty())
                {
                    tvTaskList.Nodes.SetStyleForSearch(searchText, _treeNodeDataCachingService);
                }
            }
            
            return Task.FromResult(new BaseResponse());
        }
    }
}