using InformationTree.Infrastructure.MediatR.Test.Events;
using MediatR;

namespace InformationTree.Infrastructure.MediatR.Test.Handlers.EventHandlers;

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
