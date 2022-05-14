using InformationTree.Infrastructure.MediatR.Test.Events;
using MediatR;

namespace InformationTree.Infrastructure.MediatR.Test.Handlers.EventHandlers;

public class PingedAlsoHandler : INotificationHandler<Pinged>
{
    private readonly TextWriter _writer;

    public PingedAlsoHandler(TextWriter writer)
    {
        _writer = writer;
    }

    public Task Handle(Pinged notification, CancellationToken cancellationToken)
    {
        return _writer.WriteLineAsync("Got pinged also async.");
    }
}
