using MediatR;
using MediatR.Pipeline;

namespace InformationTree.Infrastructure.MediatR.Test.Handlers.PostProcessors;

public class GenericRequestPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly TextWriter _writer;

    public GenericRequestPostProcessor(TextWriter writer)
    {
        _writer = writer;
    }

    public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        return _writer.WriteLineAsync("- All Done");
    }
}
