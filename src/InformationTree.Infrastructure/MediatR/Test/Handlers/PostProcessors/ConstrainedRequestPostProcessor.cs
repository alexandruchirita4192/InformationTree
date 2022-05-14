using InformationTree.Infrastructure.MediatR.Test.Requests;
using MediatR;
using MediatR.Pipeline;

namespace InformationTree.Infrastructure.MediatR.Test.Handlers.PostProcessors;

public class ConstrainedRequestPostProcessor<TRequest, TResponse>
    : IRequestPostProcessor<TRequest, TResponse>
where TRequest : Ping, IRequest<TResponse>
{
    private readonly TextWriter _writer;

    public ConstrainedRequestPostProcessor(TextWriter writer)
    {
        _writer = writer;
    }

    public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        return _writer.WriteLineAsync("- All Done with Ping");
    }
}