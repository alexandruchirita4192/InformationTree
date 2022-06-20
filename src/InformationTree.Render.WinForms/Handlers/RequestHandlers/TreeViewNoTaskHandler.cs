using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers;

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

        // After select with a null selected node should set everything up perfectly
        tvTaskList.SelectedNode = null;
        
        return await _mediator.Send(request.AfterSelectRequest, cancellationToken);
    }
}