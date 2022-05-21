using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;

namespace InformationTree.Render.WinForms.Handlers.PostProcessors
{
    public class EmptyPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            // Intentionately empty because a postprocessor is required for MediatR to work.
            return Task.CompletedTask;
        }
    }
}