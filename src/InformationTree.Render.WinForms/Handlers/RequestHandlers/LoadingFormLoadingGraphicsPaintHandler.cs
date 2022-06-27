using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class LoadingFormLoadingGraphicsPaintHandler : IRequestHandler<LoadingFormLoadingGraphicsPaintRequest, BaseResponse>
    {
        public Task<BaseResponse> Handle(LoadingFormLoadingGraphicsPaintRequest request, CancellationToken cancellationToken)
        {
            if (request.Graphics is not Graphics graphics)
                return Task.FromResult<BaseResponse>(null);
            if (request.GraphicsFile == null)
                return Task.FromResult<BaseResponse>(null);

            request.GraphicsFile.Show(graphics);

            return Task.FromResult(new BaseResponse());
        }
    }
}