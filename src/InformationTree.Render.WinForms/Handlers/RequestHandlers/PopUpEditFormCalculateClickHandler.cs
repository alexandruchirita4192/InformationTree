using System.Threading;
using System.Threading.Tasks;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class PopUpEditFormCalculateClickHandler : IRequestHandler<PopUpEditFormCalculateClickRequest, BaseResponse>
    {
        private readonly IPopUpService _popUpService;

        public PopUpEditFormCalculateClickHandler(IPopUpService popUpService)
        {
            _popUpService = popUpService;
        }

        public Task<BaseResponse> Handle(PopUpEditFormCalculateClickRequest request, CancellationToken cancellationToken)
        {
            // TODO: Make tbData.CalculateFunction work with a new facade, interface for calculation service which should implement basic calculation functions

            _popUpService.ShowInfo("This feature is not finished yet. It will be available at some point in a next version.");

            return Task.FromResult(new BaseResponse());
        }
    }
}