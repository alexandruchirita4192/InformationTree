using System;
using System.Threading;
using System.Threading.Tasks;
using InformationTree.Domain.Requests.Base;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR.Pipeline;

namespace InformationTree.Render.WinForms.Handlers.ExceptionHandlers
{
    public class PopUpExceptionHandler : AsyncRequestExceptionHandler<BaseRequest, BaseResponse>
    {
        private readonly IPopUpService _popUpService;

        public PopUpExceptionHandler(IPopUpService popUpService)
        {
            _popUpService = popUpService;
        }

        protected override Task Handle(BaseRequest request, Exception exception, RequestExceptionHandlerState<BaseResponse> state, CancellationToken cancellationToken)
        {
            _popUpService.ShowWarning($"{exception.Message}{Environment.NewLine}{Environment.NewLine}Check log file Log.txt", "Error occured");
            state.SetHandled(new BaseResponse());
            return Task.CompletedTask;
        }
    }
}