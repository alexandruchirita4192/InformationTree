using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Render.WinForms.Extensions;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers;

public class SetControlCursorHandler : IRequestHandler<SetControlCursorRequest, BaseResponse>
{
    public Task<BaseResponse> Handle(SetControlCursorRequest request, CancellationToken cancellationToken)
    {
        if (request.Control is Control control)
            control.InvokeWrapper(t =>
            {
                var cursor = request.IsWaitCursor ? Cursors.WaitCursor : Cursors.Default;
                t.Cursor = cursor;
            });
        return Task.FromResult(new BaseResponse());
    }
}