using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;

namespace InformationTree.Render.WinForms.Handlers.PreProcessors
{
    public class EmptyRequestPreProcessor<TRequest> : IRequestPreProcessor<TRequest>
    {
        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            // Intentionately empty because a preprocessor is required for MediatR to work.
            return Task.CompletedTask;
        }
    }
}