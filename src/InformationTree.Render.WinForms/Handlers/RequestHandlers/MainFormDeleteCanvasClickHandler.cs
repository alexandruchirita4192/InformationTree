using System.Threading;
using System.Threading.Tasks;
using InformationTree.Domain;
using InformationTree.Domain.Entities.Graphics;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormDeleteCanvasClickHandler : IRequestHandler<MainFormDeleteCanvasClickRequest, BaseResponse>
    {
        private readonly ICachingService _cachingService;

        public MainFormDeleteCanvasClickHandler(ICachingService cachingService)
        {
            _cachingService = cachingService;
        }

        public Task<BaseResponse> Handle(MainFormDeleteCanvasClickRequest request, CancellationToken cancellationToken)
        {
            var canvasForm = _cachingService.Get<ICanvasForm>(Constants.CacheKeys.CanvasForm);
            
            if (canvasForm == null || canvasForm.IsDisposed)
                return Task.FromResult<BaseResponse>(null);
            
            canvasForm.Close();
            canvasForm.Dispose();
            canvasForm = null;
            
            _cachingService.Set(Constants.CacheKeys.CanvasForm, canvasForm);

            return Task.FromResult(new BaseResponse());
        }
    }
}