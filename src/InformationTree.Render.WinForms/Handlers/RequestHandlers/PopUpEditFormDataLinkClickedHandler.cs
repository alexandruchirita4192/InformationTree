using System.Threading;
using System.Threading.Tasks;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class PopUpEditFormDataLinkClickedHandler : IRequestHandler<PopUpEditFormDataLinkClickedRequest, BaseResponse>
    {
        public Task<BaseResponse> Handle(PopUpEditFormDataLinkClickedRequest request, CancellationToken cancellationToken)
        {
            if (request.LinkText.IsEmpty())
                return Task.FromResult<BaseResponse>(null);

            System.Diagnostics.Process.Start(request.LinkText);
            
            return Task.FromResult(new BaseResponse());
        }
    }
}