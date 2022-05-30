using System;
using InformationTree.Domain.Requests.Base;
using MediatR.Pipeline;
using NLog;

namespace InformationTree.Render.WinForms.Handlers.ExceptionHandlers
{
    public class LogExceptionAction : RequestExceptionAction<BaseRequest>
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected override void Execute(BaseRequest request, Exception exception)
        {
            _logger.Error("Error occured while processing MediatR request '{0}':\n'{1}'",
                request.GetType().Name,
                exception.ToString());
        }
    }
}