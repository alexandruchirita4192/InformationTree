using System.Threading;
using System.Threading.Tasks;
using InformationTree.Domain;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormKeyUpHandler : IRequestHandler<MainFormKeyUpRequest, BaseResponse>
    {
        private readonly ICachingService _cachingService;

        public MainFormKeyUpHandler(ICachingService cachingService)
        {
            _cachingService = cachingService;
        }

        public Task<BaseResponse> Handle(MainFormKeyUpRequest request, CancellationToken cancellationToken)
        {
            _cachingService.Set(Constants.CacheKeys.IsControlKeyPressed, false);

            return Task.FromResult(new BaseResponse());
        }
    }
}