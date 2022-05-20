using System;
using InformationTree.Domain.Requests;
using MediatR.Pipeline;
using NLog;

namespace InformationTree.Render.WinForms.Handlers.ExceptionHandlers
{
    public class LogExceptionAction : RequestExceptionAction<BaseRequest>
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected override void Execute(BaseRequest request, Exception exception)
        {
            _logger.Error(exception, "Error while processing MediatR request {0}", request.GetType().Name);
        }
    }
}