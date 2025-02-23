using System;
using System.Threading;
using System.Threading.Tasks;
using InformationTree.Domain.Requests.Base;
using InformationTree.Domain.Services;
using MediatR.Pipeline;

namespace InformationTree.Render.WinForms.Handlers.ExceptionHandlers
{
    public class PopUpExceptionHandler : IRequestExceptionAction<BaseRequest, Exception>
    {
        private readonly IPopUpService _popUpService;

        public PopUpExceptionHandler(IPopUpService popUpService)
        {
            _popUpService = popUpService;
        }

        public Task Execute(BaseRequest request, Exception exception, CancellationToken cancellationToken)
        {
            _popUpService.ShowWarning($"{exception.Message}{Environment.NewLine}{Environment.NewLine}Check log file Log.txt", "Error occured");
            return Task.CompletedTask;
        }
    }
}