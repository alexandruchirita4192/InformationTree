using System;
using System.Threading.Tasks;
using System.Threading;
using InformationTree.Domain.Requests.Base;
using MediatR.Pipeline;
using NLog;

namespace InformationTree.Render.WinForms.Handlers.ExceptionHandlers
{
    public class LogExceptionAction : IRequestExceptionAction<BaseRequest, Exception>
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Task Execute(BaseRequest request, Exception exception, CancellationToken cancellationToken)
        {
            _logger.Error("Error occured while processing MediatR request '{0}':\n'{1}'",
                request.GetType().Name,
                exception.ToString());

            return Task.CompletedTask;
        }
    }
}