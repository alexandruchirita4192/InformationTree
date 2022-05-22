using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Render.WinForms.Extensions;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers;

public class TreeViewCollapseAndRefreshHandler : IRequestHandler<TreeViewCollapseAndRefreshRequest, BaseResponse>
{
    public Task<BaseResponse> Handle(TreeViewCollapseAndRefreshRequest request, CancellationToken cancellationToken)
    {
        if (request.TreeView is TreeView treeView)
            treeView.InvokeWrapper(tv =>
            {
                tv.CollapseAll();
                tv.Refresh();
            });
        return Task.FromResult(new BaseResponse());
    }
}