using System.Threading;
using System.Threading.Tasks;
using InformationTree.Domain.Requests.Base;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;
using MediatR.Pipeline;

namespace InformationTree.Render.WinForms.Handlers.PostProcessors
{
    // Note: Do not delete profiling PostProcessor even if it's not required or used
    // because a PostProcessor is still required in order for MediatR to work
    public class ProfilingPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
        where TRequest : BaseRequest, IRequest<TResponse>
        where TResponse : BaseResponse
    {
        private readonly IProfilingService _profilingService;
        private readonly IConfigurationReader _configurationReader;

        public ProfilingPostProcessor(
            IProfilingService profilingService,
            IConfigurationReader configurationReader)
        {
            _profilingService = profilingService;
            _configurationReader = configurationReader;
        }

        public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            if (_configurationReader.GetConfiguration().ApplicationFeatures.EnableProfiling == false)
                return Task.CompletedTask;

            _profilingService.EndProfiling(request, response);

            return Task.CompletedTask;
        }
    }
}