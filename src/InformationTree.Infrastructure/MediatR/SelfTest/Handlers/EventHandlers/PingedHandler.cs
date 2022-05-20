using InformationTree.Infrastructure.MediatR.SelfTest.Events;
using MediatR;

namespace InformationTree.Infrastructure.MediatR.SelfTest.Handlers.EventHandlers;

public class PingedHandler : INotificationHandler<Pinged>
{
    private readonly TextWriter _writer;

    public PingedHandler(TextWriter writer)
    {
        _writer = writer;
    }

    public Task Handle(Pinged notification, CancellationToken cancellationToken)
    {
        return _writer.WriteLineAsync("Got pinged async.");
    }
}
