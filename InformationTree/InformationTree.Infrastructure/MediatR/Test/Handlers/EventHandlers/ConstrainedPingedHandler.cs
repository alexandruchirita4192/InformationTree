using InformationTree.Infrastructure.MediatR.Test.Events;
using MediatR;

namespace InformationTree.Infrastructure.MediatR.Test.Handlers.EventHandlers;

public class ConstrainedPingedHandler<TNotification> : INotificationHandler<TNotification>
    where TNotification : Pinged
{
    private readonly TextWriter _writer;

    public ConstrainedPingedHandler(TextWriter writer)
    {
        _writer = writer;
    }

    public Task Handle(TNotification notification, CancellationToken cancellationToken)
    {
        return _writer.WriteLineAsync("Got pinged constrained async.");
    }
}
