using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Render.WinForms.Extensions;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers;

public class FormCloseHandler : IRequestHandler<FormCloseRequest, BaseResponse>
{
    public Task<BaseResponse> Handle(FormCloseRequest request, CancellationToken cancellationToken)
    {
        if (request.Form is not Form form)
            return Task.FromResult<BaseResponse>(null);

        form.InvokeWrapper(f => f.Close());

        return Task.FromResult(new BaseResponse());
    }
}