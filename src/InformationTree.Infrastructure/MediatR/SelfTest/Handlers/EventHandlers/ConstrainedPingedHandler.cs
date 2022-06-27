using InformationTree.Infrastructure.MediatR.SelfTest.Events;
using MediatR;

namespace InformationTree.Infrastructure.MediatR.SelfTest.Handlers.EventHandlers;

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
