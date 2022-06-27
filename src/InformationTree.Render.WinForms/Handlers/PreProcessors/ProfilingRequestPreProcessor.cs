using System.Threading;
using System.Threading.Tasks;
using InformationTree.Domain.Requests.Base;
using InformationTree.Domain.Services;
using MediatR.Pipeline;

namespace InformationTree.Render.WinForms.Handlers.PreProcessors
{
    // Note: Do not delete profiling PreProcessor even if it's not required or used
    // because a PreProcessor is still required in order for MediatR to work
    public class ProfilingRequestPreProcessor<TRequest> : IRequestPreProcessor<TRequest>
        where TRequest : BaseRequest
    {
        private readonly IProfilingService _profilingService;
        private readonly IConfigurationReader _configurationReader;

        public ProfilingRequestPreProcessor(
            IProfilingService profilingService,
            IConfigurationReader configurationReader)
        {
            _profilingService = profilingService;
            _configurationReader = configurationReader;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            if (_configurationReader.GetConfiguration().ApplicationFeatures.EnableProfiling == false)
                return Task.CompletedTask;

            _profilingService.StartProfiling(request);

            return Task.CompletedTask;
        }
    }
}