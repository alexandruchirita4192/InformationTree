using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Render.WinForms.Extensions;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class FormMouseDoubleClickHandler : IRequestHandler<FormMouseDoubleClickRequest, BaseResponse>
    {
        public Task<BaseResponse> Handle(FormMouseDoubleClickRequest request, CancellationToken cancellationToken)
        {
            if (request.Form is not Form form)
                return Task.FromResult<BaseResponse>(null);

            form.InvokeWrapper(form =>
            {
                form.FormBorderStyle =
                    form.FormBorderStyle == FormBorderStyle.Sizable
                        ? FormBorderStyle.None
                        : FormBorderStyle.Sizable;
            });

            return Task.FromResult(new BaseResponse());
        }
    }
}