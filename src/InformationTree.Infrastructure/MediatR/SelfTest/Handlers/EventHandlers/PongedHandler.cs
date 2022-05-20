using InformationTree.Infrastructure.MediatR.SelfTest.Events;
using MediatR;

namespace InformationTree.Infrastructure.MediatR.SelfTest.Handlers.EventHandlers;

public class PongedHandler : INotificationHandler<Ponged>
{
    private readonly TextWriter _writer;

    public PongedHandler(TextWriter writer)
    {
        _writer = writer;
    }

    public Task Handle(Ponged notification, CancellationToken cancellationToken)
    {
        return _writer.WriteLineAsync("Got ponged async.");
    }
}
