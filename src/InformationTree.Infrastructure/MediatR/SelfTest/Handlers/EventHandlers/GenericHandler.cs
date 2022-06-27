using MediatR;

namespace InformationTree.Infrastructure.MediatR.SelfTest.Handlers.EventHandlers;

public class GenericHandler : INotificationHandler<INotification>
{
    private readonly TextWriter _writer;

    public GenericHandler(TextWriter writer)
    {
        _writer = writer;
    }

    public Task Handle(INotification notification, CancellationToken cancellationToken)
    {
        return _writer.WriteLineAsync("Got notified.");
    }
}
