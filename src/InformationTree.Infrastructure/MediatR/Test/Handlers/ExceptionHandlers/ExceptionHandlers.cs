﻿using InformationTree.Infrastructure.MediatR.Test.Responses;
using MediatR.Pipeline;

namespace InformationTree.Infrastructure.MediatR.Test.Handlers.ExceptionHandlers;

public class CommonExceptionHandler : AsyncRequestExceptionHandler<PingResource, Pong>
{
    private readonly TextWriter _writer;

    public CommonExceptionHandler(TextWriter writer) => _writer = writer;

    protected override async Task Handle(PingResource request,
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

public class ConnectionExceptionHandler : IRequestExceptionHandler<PingResource, Pong, ConnectionException>
{
    private readonly TextWriter _writer;

    public ConnectionExceptionHandler(TextWriter writer) => _writer = writer;

    public async Task Handle(PingResource request,
        ConnectionException exception,
        RequestExceptionHandlerState<Pong> state,
        CancellationToken cancellationToken)
    {
        // Exception type name required because it is checked later in messages
        await _writer.WriteLineAsync($"Handling {exception.GetType().FullName}");

        // Exception handler type name required because it is checked later in messages
        await _writer.WriteLineAsync($"---- Exception Handler: '{typeof(ConnectionExceptionHandler).FullName}'").ConfigureAwait(false);

        state.SetHandled(new Pong());
    }
}

public class AccessDeniedExceptionHandler : IRequestExceptionHandler<PingResource, Pong, ForbiddenException>
{
    private readonly TextWriter _writer;

    public AccessDeniedExceptionHandler(TextWriter writer) => _writer = writer;

    public async Task Handle(PingResource request,
        ForbiddenException exception,
        RequestExceptionHandlerState<Pong> state,
        CancellationToken cancellationToken)
    {
        // Exception type name required because it is checked later in messages
        await _writer.WriteLineAsync($"Handling {exception.GetType().FullName}");

        // Exception handler type name required because it is checked later in messages
        await _writer.WriteLineAsync($"---- Exception Handler: '{typeof(AccessDeniedExceptionHandler).FullName}'").ConfigureAwait(false);

        state.SetHandled(new Pong());
    }
}

public class ServerExceptionHandler : IRequestExceptionHandler<PingNewResource, Pong, ServerException>
{
    private readonly TextWriter _writer;

    public ServerExceptionHandler(TextWriter writer) => _writer = writer;

    public virtual async Task Handle(PingNewResource request,
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
