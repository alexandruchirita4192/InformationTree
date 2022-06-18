using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers;

public class TreeViewExpandOrCollapseHandler : IRequestHandler<TreeViewExpandOrCollapseRequest, BaseResponse>
{
    public Task<BaseResponse> Handle(TreeViewExpandOrCollapseRequest request, CancellationToken cancellationToken)
    {
        if (request.TreeView is not TreeView tvTaskList)
            return Task.FromResult<BaseResponse>(null);

        if (request.ChangeType == ExpandOrCollapseChangeType.ExpandAll)
        {
            tvTaskList.ExpandAll();
        }
        else if (request.ChangeType == ExpandOrCollapseChangeType.CollapseAll)
        {
            tvTaskList.CollapseAll();
        }

        return Task.FromResult(new BaseResponse());
    }
}