using InformationTree.Infrastructure.MediatR.Test.Requests;
using MediatR;

namespace InformationTree.Infrastructure.MediatR.Test.Handlers.RequestHandlers;

public class JingHandler : AsyncRequestHandler<Jing>
{
    private readonly TextWriter _writer;

    public JingHandler(TextWriter writer)
    {
        _writer = writer;
    }

    protected override Task Handle(Jing request, CancellationToken cancellationToken)
    {
        return _writer.WriteLineAsync($"--- Handled Jing: {request.Message}, no Jong");
    }
}
