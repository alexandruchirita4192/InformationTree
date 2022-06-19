using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Render.WinForms.Extensions;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class LoadingFormTimerElapsedHandler : IRequestHandler<LoadingFormTimerElapsedRequest, BaseResponse>
    {
        public Task<BaseResponse> Handle(LoadingFormTimerElapsedRequest request, CancellationToken cancellationToken)
        {
            if (request.FileLoadProgressBar is not ProgressBar pbFileLoad)
                return Task.FromResult<BaseResponse>(null);

            if (request.LoadingGraphicsPictureBox is not PictureBox pbLoadingGraphics)
                return Task.FromResult<BaseResponse>(null);

            if (pbFileLoad.Style != ProgressBarStyle.Marquee)
            {
                // Marquee style progress bar shouldn't perform steps manually
                pbFileLoad.InvokeWrapper(fl => fl.PerformStep());
            }

            pbLoadingGraphics.InvokeWrapper(lg => lg.Refresh());

            return Task.FromResult(new BaseResponse());
        }
    }
}