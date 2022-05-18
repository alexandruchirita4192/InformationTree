﻿using InformationTree.Infrastructure.MediatR.Test.Responses;
using MediatR.Pipeline;

namespace InformationTree.Infrastructure.MediatR.Test.Handlers.ExceptionHandlers.Overrides;

public class CommonExceptionHandler : AsyncRequestExceptionHandler<PingResourceTimeout, Pong>
{
    private readonly TextWriter _writer;

    public CommonExceptionHandler(TextWriter writer) => _writer = writer;

    protected override async Task Handle(PingResourceTimeout request,
        Exception exception,
        RequestExceptionHandlerState<Pong> state,
        CancellationToken cancellationToken)
    {
        // Exception type name required because it is checked later in messages
        await _writer.WriteLineAsync($"Handling {exception.GetType().FullName}");

        // Exception handler type name required because it is checked later in messages
        await _writer.WriteLineAsync($"---- Exception Handler: '{typeof(CommonExceptionHandler).FullName}'").ConfigureAwait(false);

        state.SetHandled(new Pong());
    }
}

public class ServerExceptionHandler : ExceptionHandlers.ServerExceptionHandler
{
    private readonly TextWriter _writer;

    public ServerExceptionHandler(TextWriter writer) : base(writer) => _writer = writer;

    public override async Task Handle(PingNewResource request,
        ServerException exception,
        RequestExceptionHandlerState<Pong> state,
        CancellationToken cancellationToken)
    {
        // Exception type name required because it is checked later in messages
        await _writer.WriteLineAsync($"Handling {exception.GetType().FullName}");

        // Exception handler type name required because it is checked later in messages
        await _writer.WriteLineAsync($"---- Exception Handler: '{typeof(ServerExceptionHandler).FullName}'").ConfigureAwait(false);

        state.SetHandled(new Pong());
    }
}